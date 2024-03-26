using System.Collections;
using FileMapping.PInvoke;
using static FileMapping.PInvoke.Win32Api;

namespace FileMapping.SectorAnalysis.PhysicalDisk;

// todo 响应重新插拔的事件
internal sealed class PhysicalDiskFactory : IEnumerable<PhysicalDiskGpt>
{
	private static PhysicalDiskFactory? _instance;
	private static readonly List<PhysicalDiskGpt> _trackList = [];

	private PhysicalDiskFactory() { }

	internal static PhysicalDiskFactory Instance => _instance ??= new PhysicalDiskFactory();

	public IEnumerator<PhysicalDiskGpt> GetEnumerator()
	{
		_trackList.Clear();
		for (ulong i = 0;; ++i)
		{
			var physicalDiskHandle = CreateFileW(@$"\\.\PhysicalDrive{i}",
				DesiredAccess.GenericRead, FileShare.Read, ref NullSecurityAttributes, FileMode.Open,
				FileFlagsAndAttributes.NoBuffering | FileFlagsAndAttributes.WriteThrough, nint.Zero
			);
			if (physicalDiskHandle.IsInvalid) yield break;
			PhysicalDiskGpt? disk;
			try
			{
				disk = new PhysicalDiskGpt(i, physicalDiskHandle);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.Message);
				continue;
			}
			_trackList.Add(disk);
			yield return disk;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	// todo 待重构，这里仅仅是演示响应
}