using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FileMapping.UsnOperation;
using Microsoft.Win32.SafeHandles;

[assembly: DisableRuntimeMarshalling]

namespace FileMapping.PInvoke;

internal static partial class Win32Api
{
	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool GetVolumeInformationW(
		string lpRootPathName,
		[Out] char[] lpVolumeNameBuffer,
		uint nVolumeNameSize,
		out uint lpVolumeSerialNumber,
		out uint lpMaximumComponentLength,
		out uint lpFileSystemFlags,
		[Out] char[] lpFileSystemNameBuffer,
		uint nFileSystemNameSize);

	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
	internal static partial SafeFileHandle CreateFileW(
		string lpFileName,
		DesiredAccess dwDesiredAccess,
		FileShare dwShareMode,
		IntPtr lpSecurityAttributes,
		FileMode dwCreationDisposition,
		FileAttributes dwFlagsAndAttributes,
		IntPtr hTemplateFile);

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool DeviceIoControl(
		SafeFileHandle hDevice,
		FileSystemControlCode dwIoControlCode,
		IntPtr lpInBuffer,
		uint nInBufferSize,
		IntPtr lpOutBuffer,
		uint nOutBufferSize,
		out uint lpBytesReturned,
		IntPtr lpOverlapped
	);
}