using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.DiskGeometry;

[StructLayout(LayoutKind.Sequential)]
public readonly struct DiskGeometry
{
	internal readonly long Cylinders;
	internal readonly MediaType MediaType;
	internal readonly uint TracksPerCylinder;
	internal readonly uint SectorsPerTrack;
	internal readonly uint BytesPerSector;
}