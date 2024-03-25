using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace FileMapping.PInvoke;

internal static partial class Win32Api
{
	internal static SecurityAttributes? NullSecurityAttributes = null;
	internal static NativeOverlapped? NullNativeOverlapped = null;

	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
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
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
	internal static partial SafeFileHandle CreateFileW(
		string lpFileName,
		DesiredAccess dwDesiredAccess,
		FileShare dwShareMode,
		ref SecurityAttributes? lpSecurityAttributes,
		FileMode dwCreationDisposition,
		FileFlagsAndAttributes dwFlagsAndAttributes,
		IntPtr hTemplateFile);

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool DeviceIoControl(
		SafeFileHandle hDevice,
		IoControlCodes dwIoControlCodes,
		IntPtr lpInBuffer,
		uint nInBufferSize,
		IntPtr lpOutBuffer,
		uint nOutBufferSize,
		out uint lpBytesReturned,
		ref NativeOverlapped? lpOverlapped
	);

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial bool SetFilePointerEx(
		SafeFileHandle hFile,
		ulong liDistanceToMove,
		out IntPtr lpNewFilePointer,
		MoveMethod dwMoveMethod
	);

	internal static IntPtr? SetFilePointerEx(
		SafeFileHandle hFile,
		long liDistanceToMove,
		MoveMethod dwMoveMethod
	) =>
		// 移动方法为 Begin 时，移动距离会被诠释成无符号数，其余情况均是有符号
		SetFilePointerEx(hFile, unchecked((ulong)liDistanceToMove), out var newPointer, dwMoveMethod)
			? newPointer
			: null;

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool ReadFile(
		SafeFileHandle hFile,
		[Out] byte[] lpBuffer,
		uint nNumberOfBytesToRead,
		out uint lpNumberOfBytesRead,
		ref NativeOverlapped? lpOverlapped
	);

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool ReadFileEx(
		SafeFileHandle hFile,
		[Out] byte[] lpBuffer,
		uint nNumberOfBytesToRead,
		ref NativeOverlapped? lpOverlapped,
		IntPtr lpCompletionRoutine
	);
}