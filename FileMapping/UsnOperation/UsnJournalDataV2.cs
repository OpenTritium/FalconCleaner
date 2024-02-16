using System.Runtime.InteropServices;

namespace FileMapping.UsnOperation;

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct UsnJournalDataV2
{
    internal readonly ulong UsnJournalId;
    internal readonly long FirstUsn;
    internal readonly long NextUsn;
    internal readonly long LowestValidUsn;
    internal readonly long MaxUsn;
    internal readonly ulong MaximumSize;
    internal readonly ulong AllocationDelta;
    internal readonly ushort MinSupportedMajorVersion;
    internal readonly ushort MaxSupportedMajorVersion;
    internal readonly uint Flags;
    internal readonly ulong RangeTrackChunkSize;
    internal readonly long RangeTrackFileSizeThreshold;  // 大于等于才会触发输出 v4 记录
}