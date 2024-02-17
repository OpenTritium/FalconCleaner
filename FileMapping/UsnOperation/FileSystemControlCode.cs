namespace FileMapping.UsnOperation;

internal enum FileSystemControlCode : uint
{
	QueryUsnJournal = 0x900F4,
	CreateUsnJournal = 0x900E7,
	DeleteUsnJournal = 0x900F8,
	EnumUsnData = 0x900B3
}