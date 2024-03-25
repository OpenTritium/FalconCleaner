using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DriveLayoutInformationEx
{
	internal readonly PartitionTableStyle PartitionStyle;
	internal readonly uint PartitionCount;
	internal readonly DriveLayoutInformationUnion DummyUnionName;
	internal readonly PartitionInformationEx PartitionEntry;
}