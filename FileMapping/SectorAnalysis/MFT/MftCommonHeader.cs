namespace FileMapping.SectorAnalysis.MFT;

public unsafe struct MftCommonHeader
{
	internal fixed byte Signature[4];
	internal ushort UsnOffset;
	internal ushort UsnLength;
	internal ulong Lsn; // 日志序列号
	internal ushort Run; // 记录使用号
	internal ushort HardLinkCount;
	internal ushort FirstAttributeOffset;
	internal ushort Flag;//todo 记得做成枚举
	internal ushort EntryUseLength;
	internal ushort EntryAllocatedLength;
}