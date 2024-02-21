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