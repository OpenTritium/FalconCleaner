using System.Runtime.InteropServices;

namespace FileMapping.UsnOperation;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct UsnRecordCommonHeader
{
	internal readonly uint RecordLength;
	internal readonly ushort MajorVersion;
	internal readonly ushort MinorVersion;
}