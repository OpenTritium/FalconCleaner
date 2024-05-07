namespace FileMapping.SectorAnalysis.MFT;

public unsafe struct MftCommonHeader
{
	internal fixed byte Signature[4];// 可用则 FILE 不可用则BAAD
	internal ushort UsnOffset;//0x2A XP之前，0x30 XP以后
	internal ushort UsnLength;//双字节数量，通常为3 两个扇区末尾两字节和usn
	internal ulong Lsn; // 日志序列号，每次记录修改后改变
	internal ushort Run; // 记录使用的次数
	internal ushort HardLinkCount;
	internal ushort FirstAttributeOffset;
	internal ushort Flag;//todo 记得做成枚举
	internal ushort EntryActualLength;
	internal ushort EntryAllocatedLength;
	internal ulong BaseRecordReference;// 是基本记录则为0
	internal byte NextAttributeId;
	internal ushort Reserved;
	internal uint ClusterOffset;
	internal ulong FixUp;
}