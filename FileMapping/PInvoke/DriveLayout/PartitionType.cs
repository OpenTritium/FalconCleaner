// ReSharper disable IdentifierTypo

namespace FileMapping.PInvoke.DriveLayout;

internal static class PartitionType
{
	internal static Guid BasicData { get; } = new("ebd0a0a2-b9e5-4433-87c0-68b6b72699c7");
	internal static Guid EntryUnused { get; } = new("00000000-0000-0000-0000-000000000000");
	internal static Guid System { get; } = new("c12a7328-f81f-11d2-ba4b-00a0c93ec93b");
	internal static Guid MsftReserved { get; } = new("e3c9e316-0b5c-4db8-817d-f92df00215ae");
	internal static Guid LdmMetadata { get; } = new("5808c8aa-7e8f-42e0-85d2-e1e90434cfb3");
	internal static Guid LdmData { get; } = new("af9b60a0-1431-4f62-bc68-3311714a69ad");
	internal static Guid MsftRecovery { get; } = new("de94bba4-06d1-4d40-a16a-bfd50179d6ac");
}