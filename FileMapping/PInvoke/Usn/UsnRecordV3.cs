using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.Usn;

[StructLayout(LayoutKind.Sequential)]
internal struct UsnRecordV3
{
	internal readonly UsnRecordCommonHeader Header;
	internal unsafe fixed ulong FileReferenceNumber[2];
	internal unsafe fixed ulong ParentFileReferenceNumber[2];
	internal readonly long Usn;
	internal readonly long TimeStamp;
	internal readonly uint Reason;
	internal readonly uint SourceInfo;
	internal readonly uint SecurityId;
	internal readonly uint FileAttributes;
	internal readonly ushort FileNameLength;
	internal readonly ushort FileNameOffset;
	internal unsafe fixed char FileName[1]; // todo 废弃
}