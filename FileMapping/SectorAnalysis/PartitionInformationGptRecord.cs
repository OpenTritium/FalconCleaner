namespace FileMapping.SectorAnalysis;

internal record PartitionInformationGptRecord
{
	internal required long StartingOffset { get; init; }
	internal required long PartitionLength { get; init; }
	internal required uint PartitionNumber { get; init; }
	internal required bool RewritePartition { get; init; }
	internal required bool IsServicePartition { get; init; }
	internal required Guid PartitionType { get; init; }
	internal required Guid PartitionId { get; init; }
	internal required ulong Attributes { get; init; }
	internal required string Name { get; init; }
}