using FileMapping.RawDiskProber;
using Microsoft.Win32.SafeHandles;

namespace FileMapping.Test;

[TestClass]
public sealed class RawDiskProber
{
	[TestMethod]
	[DataRow(0UL)]
	// N 为当前硬盘数 - 1
	public void CreatePhysicalDiskHandle(ulong physicalDiskCount)
	{
		using var hDevice = RawDiskUtils.GetPhysicalDiskHandle(physicalDiskCount);
		Assert.IsNotNull(hDevice,"申请物理硬盘句柄失效");
	}
	
	[TestMethod]
	[DataRow(0)]
	[DataRow(255)]
	public void MoveFileHandle(long distance)
	{
		using var hDisk = RawDiskUtils.GetPhysicalDiskHandle(1);
		Assert.IsNotNull(hDisk);
		Assert.IsTrue(RawDiskUtils.ShiftPointerForward(hDisk, distance, out _),"移动文件指针失败");
	}
}