﻿using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.USN;

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct CreateUsnJournalData
{
	internal ulong MaximumSize { get; init; }
	internal ulong AllocationDelta { get; init; }
}