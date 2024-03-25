using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DiskGeometry;

[StructLayout(LayoutKind.Sequential)]
public struct DiskGeometryEx
{
	internal readonly DiskGeometry geometry;
	internal readonly long DiskSize;
	internal unsafe fixed byte Data[1];
}