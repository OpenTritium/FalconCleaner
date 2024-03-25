using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct PartitionInformationEx
{
	internal readonly PartitionTableStyle partitionStyle;
	internal readonly long StartingOffset;
	internal readonly long PartitionLength;
	internal readonly uint PartitionNumber;
	internal readonly bool RewritePartition;
	internal readonly bool IsServicePartition; // NTDDI_VERSION >= NTDDI_WIN10_RS3
	internal readonly PartitionInformationUnion DummyUnionName;
}