namespace FileMapping.PInvoke.DriveLayout;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DriveLayoutInformationMbr
{
	internal readonly uint Signature;
	internal readonly uint CheckSum;
}