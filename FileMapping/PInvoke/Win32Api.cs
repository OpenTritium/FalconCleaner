using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace FileMapping.PInvoke;

internal static partial class Win32Api
{

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
		IntPtr lpSecurityAttributes,
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
		IntPtr lpOverlapped
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

	/// <summary>
	/// 移动文件句柄内的文件指针
	/// </summary>
	/// <param name="hFile">文件句柄</param>
	/// <param name="liDistanceToMove">移动距离，移动方法为 <seealso cref="MoveMethod.Begin"/> 时，移动距离会被诠释成无符号数，其余情况均是有符号</param>
	/// <param name="dwMoveMethod">移动方向</param>
	/// <returns>若移动成功，返回新的文件指针位置。失败则为空</returns>
	internal static IntPtr? SetFilePointerEx(
		SafeFileHandle hFile,
		ulong liDistanceToMove,
		MoveMethod dwMoveMethod
	) =>
		SetFilePointerEx(hFile, liDistanceToMove, out var newPointer, dwMoveMethod)
			? newPointer
			: null;

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool ReadFile(
		SafeFileHandle hFile,
		IntPtr lpBuffer,
		uint nNumberOfBytesToRead,
		out uint lpNumberOfBytesRead,
		IntPtr lpOverlapped
	);

	[LibraryImport("kernel32.dll", SetLastError = true)]
	[UnmanagedCallConv(CallConvs = [ typeof(CallConvStdcall) ])]
	[return: MarshalAs(UnmanagedType.Bool)]
	internal static partial bool ReadFileEx(
		IntPtr hFile,
		[Out] byte[] lpBuffer,
		uint nNumberOfBytesToRead,
		IntPtr lpOverlapped,
		IntPtr lpCompletionRoutine
	);
}