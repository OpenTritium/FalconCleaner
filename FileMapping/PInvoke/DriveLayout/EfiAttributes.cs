namespace FileMapping.PInvoke.DriveLayout;

public enum EfiAttributes : ulong
{
	GptAttributePlatformRequired = 0x0000000000000001,
	GptBasicDataAttributeNoDriveLetter = 0x8000000000000000, 
	GptBasicDataAttributeHidden = 0x4000000000000000,
	GptBasicDataAttributeShadowCopy = 0x2000000000000000,
	GptBasicDataAttributeReadOnly = 0x1000000000000000
}