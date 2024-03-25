using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

// todo 使用属性改善
[StructLayout(LayoutKind.Sequential)]
public readonly struct PartitionInformationMbr
{
	internal readonly byte PartitionType; // todo 分区枚举
	internal readonly bool BootIndicator;
	internal readonly bool RecognizedPartition;
	internal readonly uint HiddenSectors;
	internal readonly Guid PartitionId;
}