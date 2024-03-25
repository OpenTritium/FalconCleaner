using FileMapping.PInvoke;
using FileMapping.PInvoke.Usn;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Runtime.InteropServices;
using static FileMapping.PInvoke.Win32Api;

namespace FileMapping.UsnOperation;

using UsnRecord = (UInt128? currentFileRef, UInt128? parentFileRef, string? fileName);
using VolumeDescribe = (string? volumeName, string? fileSystemName, uint? serialNumber, uint? fileSytemFlags);

// todo 全局通过 FILE_FLAG_OVERLAPPED 支持异步
// 释放安全句柄
internal static class UsnUtils
{
	internal static VolumeDescribe GetVolumeInfo(char driveLetter)
	{
		// todo 无 MAX_PATH + 1 限制
		const uint maxBufferLen = 261;
		var volumeNameBuffer = new char[maxBufferLen];
		var fileSystemNameBuffer = new char[maxBufferLen];
		var rootPath = $@"{driveLetter}:\";
		if (GetVolumeInformationW(rootPath, volumeNameBuffer,
			    maxBufferLen, out var serialNumber, out _,
			    out var fileSystemFlags, fileSystemNameBuffer, maxBufferLen))
			return (new string(volumeNameBuffer), new string(fileSystemNameBuffer), serialNumber, fileSystemFlags);
		return (null, null, null, null);
	}

	internal static SafeFileHandle? GetVolumeHandle(char driveLetter)
	{
		var volumeHandle = CreateFileW(@$"\\.\{driveLetter}:", DesiredAccess.ReadWrite,
			FileShare.ReadWrite, ref NullSecurityAttributes, FileMode.Open, FileFlagsAndAttributes.Readonly,
			IntPtr.Zero
		);
		return volumeHandle.IsInvalid ? null : volumeHandle;
	}

	// 启动创建日志流，并不代表创建好了
	internal static unsafe bool CreateUsnJournal(SafeFileHandle volumeHandle)
	{
		var requestCreateUsnJournal = new CreateUsnJournalData
		{
			AllocationDelta = 0x400_0000, // 512 MB
			MaximumSize = 0x2000_0000 // 64 MB
		};
		return DeviceIoControl(volumeHandle, IoControlCodes.CreateUsnJournal,
			new IntPtr(&requestCreateUsnJournal),
			(uint)sizeof(CreateUsnJournalData), IntPtr.Zero, 0, out _, ref NullNativeOverlapped
		);
	}

	internal static unsafe bool QueryUsnJournal(SafeFileHandle volumeHandle,
		out UsnJournalDataV2 responseUsnJournalData)
	{
		UsnJournalDataV2 responseJournalData = default;
		if (DeviceIoControl(volumeHandle, IoControlCodes.QueryUsnJournal, IntPtr.Zero, 0,
			    new IntPtr(&responseJournalData), (uint)sizeof(UsnJournalDataV2), out _,
			    ref NullNativeOverlapped
		    ))
		{
			responseUsnJournalData = responseJournalData;
			return true;
		}

		responseUsnJournalData = default;
		return false;
	}

	internal sealed unsafe class UsnRecordEnumerable(SafeFileHandle volumeHandle, long nextUsn)
		: IEnumerator<UsnRecord>,
			IEnumerable<UsnRecord>
	{
		private const uint BufferSize = 0x400_0000;

		private uint _validBytesCount;
		private static readonly IntPtr BufferPointer = (IntPtr)NativeMemory.Alloc(BufferSize);
		private static IntPtr _currentRecordPointer;

		private static uint GetCurrentRecordLength() => ((UsnRecordCommonHeader*)_currentRecordPointer)->RecordLength;

		private static UsnRecordCommonHeader* GetHeaderViewOfCurrentRecord() =>
			(UsnRecordCommonHeader*)_currentRecordPointer;

		private bool WriteBufferAndUpdateBytes(MftEnumDataV1 requestEnumData)
		{
			if (DeviceIoControl(volumeHandle, IoControlCodes.EnumUsnData,
				    new IntPtr(&requestEnumData),
				    (uint)sizeof(MftEnumDataV1), BufferPointer, BufferSize, out var bytesReturned,
				    ref NullNativeOverlapped))
			{
				_validBytesCount = bytesReturned - sizeof(long); // 前 8 字节就是 USN
				return true;
			}

			_validBytesCount = 0;
			return false;
		}

		public bool MoveNext()
		{
			if (_validBytesCount != 0)
			{
				_currentRecordPointer = IntPtr.Add(_currentRecordPointer, (int)GetCurrentRecordLength());
				return true;
			}

			var requestEnumData = new MftEnumDataV1
			{
				HighUsn = nextUsn,
				LowUsn = 0,
				MaxMajorVersion = 4,
				MinMajorVersion = 3,
				StartFileReferenceNumber = *(ulong*)BufferPointer // 前 8 字节就是 USN
			};
			if (WriteBufferAndUpdateBytes(requestEnumData))
			{
				_currentRecordPointer = IntPtr.Add(BufferPointer, sizeof(ulong)); // 前 8 字节就是 USN
				return true;
			}

			_currentRecordPointer = IntPtr.Zero;
			return false;
		}

		public void Reset() => throw new NotImplementedException();

		object IEnumerator.Current => Current;

		public UsnRecord Current
		{
			get
			{
				_validBytesCount -= GetCurrentRecordLength();
				UsnRecord info = (null, null, null);
				/* 出于性能考虑，一组 V4 记录后面往往会跟随一个 V3 记录。
				 因为 v4 记录体积小，可以在文件系统忙碌时（大文件）更效率地创建，
				而尾随的 V3 记录则是总结。
				 */
				switch (GetHeaderViewOfCurrentRecord()->MajorVersion, GetHeaderViewOfCurrentRecord()->MinorVersion)
				{
					case (3, 0):
						var usnRecordV3Pointer = (UsnRecordV3*)_currentRecordPointer;
						info.fileName = Marshal.PtrToStringUni(
							IntPtr.Add(_currentRecordPointer, usnRecordV3Pointer->FileNameOffset),
							usnRecordV3Pointer->FileNameLength / sizeof(char));
						info.currentFileRef = new UInt128(lower: usnRecordV3Pointer->FileReferenceNumber[0],
							upper: usnRecordV3Pointer->FileReferenceNumber[1]);
						info.parentFileRef = new UInt128(lower: usnRecordV3Pointer->ParentFileReferenceNumber[0],
							upper: usnRecordV3Pointer->ParentFileReferenceNumber[1]);
						break;
					case (4, 0):
						var usnRecordV4Pointer = (UsnRecordV4*)_currentRecordPointer;
						info.currentFileRef = new UInt128(lower: usnRecordV4Pointer->FileReferenceNumber[0],
							upper: usnRecordV4Pointer->FileReferenceNumber[1]);
						info.parentFileRef = new UInt128(lower: usnRecordV4Pointer->ParentFileReferenceNumber[0],
							upper: usnRecordV4Pointer->ParentFileReferenceNumber[1]);
						break;
					default:
						Console.WriteLine("不受支持的版本号");
						break;
				}

				return info;
			}
		}

		public void Dispose()
		{
			NativeMemory.Free(BufferPointer.ToPointer());
		}

		public IEnumerator<UsnRecord> GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	internal static unsafe bool DeleteUsnJournal(SafeFileHandle volumeHandle, UsnJournalDataV2 responseJournalData)
	{
		var requestDeleteUsnJournal = new DeleteUsnJournalData
		{
			DeleteFlags = UsnDeleteFlags.DeleteAndNotify,
			UsnJournalId = responseJournalData.UsnJournalId
		};

		return DeviceIoControl(volumeHandle, IoControlCodes.DeleteUsnJournal,
			new IntPtr(&requestDeleteUsnJournal),
			(uint)sizeof(DeleteUsnJournalData), IntPtr.Zero, 0, out _, ref NullNativeOverlapped
		);
	}
}