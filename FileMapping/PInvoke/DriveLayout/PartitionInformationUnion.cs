using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct PartitionInformationUnion
{
	[FieldOffset(0)] internal readonly PartitionInformationMbr Mbr;
	[FieldOffset(0)] internal readonly PartitionInformationGpt Gpt;
}