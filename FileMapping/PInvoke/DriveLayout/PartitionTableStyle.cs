// ReSharper disable InconsistentNaming

namespace FileMapping.PInvoke.DriveLayout;

public enum PartitionTableStyle : uint
{
	MBR,
	GPT,
	RAW
}