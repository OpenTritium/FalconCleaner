﻿using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.USN;

[StructLayout(LayoutKind.Sequential)]
internal ref struct MftEnumDataV1
{
	internal ulong StartFileReferenceNumber { get; set; }
	internal long LowUsn { get; init; }
	internal long HighUsn { get; init; }
	internal ushort MinMajorVersion { get; init; }
	internal ushort MaxMajorVersion { get; init; }
}