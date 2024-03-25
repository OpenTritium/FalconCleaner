using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DriveLayoutInformationGpt
{
	internal readonly Guid DiskId;
	internal readonly long StartingUsableOffset;
	internal readonly long UsableLength;
	internal readonly uint MaxPartitionCount; // 如果指定的最大分区计数小于 128，则会将其重置为 128。 这符合 EFI 规范
}