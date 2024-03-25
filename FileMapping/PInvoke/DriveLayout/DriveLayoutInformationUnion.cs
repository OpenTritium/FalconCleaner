using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DriveLayout;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct DriveLayoutInformationUnion
{
	[FieldOffset(0)] internal readonly DriveLayoutInformationMbr Mbr;
	[FieldOffset(0)] internal readonly DriveLayoutInformationGpt Gpt;
}