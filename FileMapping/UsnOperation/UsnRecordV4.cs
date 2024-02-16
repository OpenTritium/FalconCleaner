using System.Runtime.InteropServices;

namespace FileMapping.UsnOperation;

/// <summary>
/// 仅当启用范围跟踪且文件大小等于或大于 RangeTrackFileSizeThreshold 成员的值时，才会输出USN_RECORD_V4记录。
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct UsnRecordV4
{
    internal readonly UsnRecordCommonHeader Header;
    internal unsafe fixed ulong FileReferenceNumber[2];
    internal unsafe fixed ulong ParentFileReferenceNumber[2];
    internal readonly long Usn;
    internal readonly uint Reason;
    internal readonly uint SourceInfo;
    internal readonly uint RemainingExtents;
    internal readonly ushort NumberOfExtents;
    internal readonly ushort ExtentSize;
    internal unsafe fixed long Extents[2];
}