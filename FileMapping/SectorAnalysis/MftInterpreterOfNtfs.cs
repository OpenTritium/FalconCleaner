using System.ComponentModel;
using FileMapping.PInvoke;

namespace FileMapping.SectorAnalysis;

public sealed class MftInterpreterOfNtfs(IntPtr partitionBeginning,uint sectorSize)
{
	private readonly IntPtr PartitionBeginning = partitionBeginning;
	private readonly uint SectorSize = sectorSize;

	public void ParseBoot()
	{
		var buffer = new byte[SectorSize];
		Console.WriteLine(PartitionBeginning.ToString());
		var result = Win32Api.ReadFile(PartitionBeginning, buffer, SectorSize, out _, ref Win32Api.NullNativeOverlapped);
		try
		{
			throw new Win32Exception();
		}
		catch (Win32Exception ex)
		{
			Console.WriteLine($"{ex.Message}");
		}
		Console.WriteLine($"{result}, {buffer[511..512]}");
	}

} 