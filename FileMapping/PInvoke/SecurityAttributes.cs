﻿using System.Runtime.InteropServices;

namespace FileMapping.PInvoke;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct SecurityAttributes
{
	internal uint Length { get; init; }
	internal IntPtr SecurityDescriptor { get; init; }
	internal bool InheritHandle { get; init; }
}