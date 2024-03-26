using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct PartitionInformationGpt
{
	internal readonly Guid PartitionType;
	internal readonly Guid PartitionId;
	internal readonly EfiAttributes Attributes;
	internal unsafe fixed char Name[36];
}