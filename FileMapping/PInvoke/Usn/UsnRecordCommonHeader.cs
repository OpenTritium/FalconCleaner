using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.Usn;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct UsnRecordCommonHeader
{
	internal readonly uint RecordLength;
	internal readonly ushort MajorVersion;
	internal readonly ushort MinorVersion;
}