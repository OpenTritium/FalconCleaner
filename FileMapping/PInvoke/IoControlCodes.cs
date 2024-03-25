namespace FileMapping.PInvoke;

internal enum IoControlCodes : uint
{
	QueryUsnJournal = 0x900F4,
	CreateUsnJournal = 0x900E7,
	DeleteUsnJournal = 0x900F8,
	EnumUsnData = 0x900B3,
	GetDriveLayoutEx = 0x70050,
	GetDriveGeometryEx = 0x700A0
}