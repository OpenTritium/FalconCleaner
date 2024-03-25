namespace FileMapping.PInvoke;

[Flags]
internal enum DesiredAccess : uint
{
	None = 0,
	GenericRead = 0x80000000,
	GenericWrite = 0x40000000,
	GenericExecute = 0x20000000,
	GenericAll = 0x10000000,
	ReadWrite = GenericWrite | GenericRead
}