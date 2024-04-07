using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using FileMapping.PInvoke;
using FileMapping.SectorAnalysis.NTFS;
using static FileMapping.PInvoke.Win32Api;

namespace FileMapping.SectorAnalysis.MFT;

// todo 改造异步句柄
// todo 实现 MFT 损坏检测
public class MftStreamOfNtfs(SafeFileHandle diskHandle, uint sectorSize,long partitionStartingOffset)
{
	private readonly unsafe IntPtr _bufferPointer = (IntPtr)NativeMemory.AlignedAlloc(sectorSize, sectorSize);
	private DataSize? _recordSize;
	public unsafe void ParseBoot()
	{
		// 无缓存模式需要对齐内存
		ReadFile(diskHandle, _bufferPointer, sectorSize,out _, IntPtr.Zero);
		var bootPointer = (BootSectorLayout*)_bufferPointer;
		var off = SetFilePointerEx(diskHandle, 0, MoveMethod.Current);
		Console.WriteLine($"{bootPointer->LcnOfMft:X} {bootPointer->SectorsPerCluster:X} {sectorSize:X}");
		var mftOffset = bootPointer->LcnOfMft * bootPointer->SectorsPerCluster * sectorSize + partitionStartingOffset;
		// 里面存储的是文件段落的数量的位数，而且还是补码
		_recordSize = new DataSize(Convert.ToUInt64(Math.Pow(2, ~(unchecked((sbyte)bootPointer->ClustersPerFileSegment) - 1))));
		var of = SetFilePointerEx(diskHandle, (ulong)mftOffset, MoveMethod.Begin);
		ReadFile(diskHandle, _bufferPointer, sectorSize, out _, IntPtr.Zero);
		Console.WriteLine($"{of:X}");
		Console.WriteLine(*(byte*)_bufferPointer.ToPointer());
	}

	public unsafe void PrepareForParserRecord()
	{
		// 扩容
		NativeMemory.AlignedRealloc(_bufferPointer.ToPointer(), (uint)_recordSize!.Value.Actual, sectorSize);
	}

	public unsafe void ParseMftRecord()
	{

	}

	unsafe ~MftStreamOfNtfs()
	{
		NativeMemory.AlignedFree(_bufferPointer.ToPointer());
	}
}