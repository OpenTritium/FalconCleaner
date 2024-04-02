using System.Collections;
using System.ComponentModel;
using FileMapping.PInvoke;
using FileMapping.SectorAnalysis.PhysicalDisk;
using static FileMapping.PInvoke.Win32Api;

namespace FileMapping.SectorAnalysis;

// todo 单例
internal sealed class MftInterpreterFactory(PhysicalDiskGpt pd) : IEnumerable<MftInterpreterOfNtfs>
{
	public IEnumerator<MftInterpreterOfNtfs> GetEnumerator()
	{
		foreach (var entry in pd.PartitionEntries)
		{
			var fp = SetFilePointerEx(pd.Handle, entry.StartingOffset, MoveMethod.Begin);
			try
			{
				if(fp is null) throw new Win32Exception();
			}
			catch (Win32Exception exception)
			{
				Console.WriteLine(exception.Message);
				continue;
			}
			yield return new MftInterpreterOfNtfs(fp.Value,pd.Geometry.BytesPerSector);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}