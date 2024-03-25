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
		using var hDevice = DiskSectorReader.GetPhysicalDiskHandle(physicalDiskCount);
		Assert.IsNotNull(hDevice,"申请物理硬盘句柄失效");
	}
}