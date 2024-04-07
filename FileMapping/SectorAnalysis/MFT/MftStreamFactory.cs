using System.Collections;
using System.ComponentModel;
using FileMapping.PInvoke;
using FileMapping.PInvoke.DriveLayout;
using FileMapping.SectorAnalysis.PhysicalDisk;
using static FileMapping.PInvoke.Win32Api;

namespace FileMapping.SectorAnalysis.MFT;

// todo 单例
// todo 实现 refs
// todo 没人告诉你只需要生产NTFS分区的流对象吗
// safehandle dispose
internal sealed class MftStreamFactory(PhysicalDiskGpt pd) : IEnumerable<MftStreamOfNtfs>
{
    public IEnumerator<MftStreamOfNtfs> GetEnumerator()
    {
        foreach (var entry in pd.PartitionEntries)
        {
	        if (entry.PartitionType != PartitionType.BasicData) continue;
	        // 为不同的文件指针准备不同的句柄
	        var diskHandle = CreateFileW(@$"\\.\PhysicalDrive{pd.Id}",
		        DesiredAccess.GenericRead, FileShare.Read, IntPtr.Zero, FileMode.Open,
		        FileFlagsAndAttributes.NoBuffering | FileFlagsAndAttributes.WriteThrough, IntPtr.Zero
	        );
	        SetFilePointerEx(diskHandle, unchecked((ulong)entry.StartingOffset), MoveMethod.Begin);
	        yield return new MftStreamOfNtfs(diskHandle,pd.Geometry.BytesPerSector,entry.StartingOffset);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}