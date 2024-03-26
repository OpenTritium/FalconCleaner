using System.Runtime.InteropServices;

namespace FileMapping.SectorAnalysis;

[StructLayout(LayoutKind.Explicit)]
internal struct NtfsBootSectorLayout
{
	[FieldOffset(0x00)] internal unsafe fixed byte JumpCommand[3];
	[FieldOffset(0x03)] internal ulong NtfsIdentifier;
	[FieldOffset(0x0B)] internal ushort BytesPerSector;
	[FieldOffset(0x0D)] internal byte SectorsPerCluster;
	[FieldOffset(0x0E)] internal ushort ReservedSectors;
	[FieldOffset(0x15)] internal byte MediaDescriptor;
	[FieldOffset(0x18)] internal ushort SectorsPerTrack;
	[FieldOffset(0x1A)] internal ushort NumbersOfHeads;
	[FieldOffset(0x1C)] internal uint HiddenSectors;
	[FieldOffset(0x28)] internal ulong TotalSectors;
	[FieldOffset(0x30)] internal ulong LcnOfMft;
	[FieldOffset(0x38)] internal ulong LcnOfMftMirror;
	[FieldOffset(0x40)] internal uint ClustersPerFileSegment;
	[FieldOffset(0x44)] internal byte ClustersPerIndexBuffer;
	[FieldOffset(0x48)] internal ulong VolumeSerialNumber;
	[FieldOffset(0x50)] internal uint CheckSum;
}