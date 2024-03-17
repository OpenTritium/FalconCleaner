using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FileMapping.PInvoke;
using FileMapping.UsnOperation;
using Microsoft.Win32.SafeHandles;

namespace FileMapping.RawDiskProber;

public static class RawDiskUtils
{
	// ni-winioctl-ioctl_disk_get_drive_geometry_ex 获取几何信息
	// IOCTL_DISK_GET_DRIVE_LAYOUT_EX 获取分区信息
	// 资源管理器最多只能显示 26 个盘符
	private const byte VisibleVolumeCount = 26;

	public static SafeFileHandle? GetPhysicalDiskHandle(ulong physicalDiskNum)
	{
		var physicalDiskHandle = Win32Api.CreateFileW(@$"\\.\PhysicalDrive{physicalDiskNum}",
			DesiredAccess.GenericRead,
			FileShare.Read,
			IntPtr.Zero, FileMode.Open, FileAttributes.ReadOnly, IntPtr.Zero
		);
		return physicalDiskHandle.IsInvalid ? null : physicalDiskHandle;
	}
	/*
	 * 需要注意的是，此函数返回的文件指针不适用于重叠读写操作。
	 * 对于重叠操作的偏移量，请使用 OVERLAPPED 结构的 Offset 和 OffsetHigh 成员。
	 * 此外，不能将 SetFilePointerEx 用于非寻址设备（如管道或通信设备）的句柄。
	 * 在多线程应用程序中设置文件指针时要小心，必须同步对共享资源的访问。
	 * 例如，如果应用程序的线程共享一个文件句柄、更新文件指针并从文件中读取数据，
	 * 应该使用临界区对象或互斥对象来保护这个序列。关于这些对象的更多信息，请参阅临界区对象和互斥对象1.
	 */
	// 偏移后的指针不需要释放
	public static bool ShiftPointerForward(SafeFileHandle currentPointer, long distance,
		out IntPtr newPointer)
	{
		if (Win32Api.SetFilePointerEx(currentPointer, distance, out var newPlace, MoveMethod.Begin))
		{
			newPointer = newPlace;
			return true;
		}

		newPointer = IntPtr.Zero;
		return false;
	}

	public delegate bool BufferParser(byte[] buffer, uint bytesCount);

	public static bool ExploitBytes(SafeFileHandle starting, uint bytesCount, BufferParser parser)
	{
		var ol = new NativeOverlapped
		{
			OffsetLow = 0,
			OffsetHigh = 0
		};

		var buffer = new byte[bytesCount];
		if (Win32Api.ReadFileEx(starting, buffer, 512,
			    ref ol,
			    IntPtr.Zero))
		{
			Console.WriteLine("有效");
			return parser(buffer, bytesCount);
		}

		Console.WriteLine("无效");
		return false;
	}
}