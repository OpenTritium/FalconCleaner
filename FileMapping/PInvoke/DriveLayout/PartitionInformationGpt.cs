using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct PartitionInformationGpt
{
	internal readonly Guid PartitionType;
	internal readonly Guid PartitionId;
	internal readonly ulong Attributes; // todo 待实现的枚举
	internal unsafe fixed char Name[36];
}