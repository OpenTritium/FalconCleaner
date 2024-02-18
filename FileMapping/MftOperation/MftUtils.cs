using FileMapping.PInvoke;
using FileMapping.UsnOperation;
using Microsoft.Win32.SafeHandles;

namespace FileMapping.MftOperation;

internal static class MftUtils
{
	// 资源管理器最多只能显示 26 个盘符
	private const byte VisibleVolumeCount = 26;

	internal static SafeFileHandle? GetPhysicalDiskHandle()
	{
		var physicalDiskHandle = Win32Api.CreateFileW(@"\\.\PhysicalDrive0", DesiredAccess.GenericRead,
			FileShare.Read,
			IntPtr.Zero, FileMode.Open, FileAttributes.ReadOnly, IntPtr.Zero
		);
		if (physicalDiskHandle.IsInvalid)
		{
			Console.WriteLine("无效");
			return null;
		}

		Console.WriteLine("有效");
		return physicalDiskHandle;
	}
}