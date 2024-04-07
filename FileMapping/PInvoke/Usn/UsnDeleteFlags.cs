namespace FileMapping.PInvoke.USN;

[Flags]
internal enum UsnDeleteFlags : uint
{
	UsnDeleteFlagDelete = 0x00000001,
	UsnDeleteFlagNotify = 0x00000002,
	DeleteAndNotify = UsnDeleteFlagDelete | UsnDeleteFlagNotify
}