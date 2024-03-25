using System.Collections;
using FileMapping.PInvoke;
using static FileMapping.PInvoke.Win32Api;

namespace FileMapping.SectorAnalysis;

// todo 响应重新插拔的事件
internal sealed class PhysicalDiskFactory : IEnumerable<PhysicalDiskGpt>
{
	private static PhysicalDiskFactory? _instance;

	private PhysicalDiskFactory() { }

	internal static PhysicalDiskFactory Instance => _instance ??= new PhysicalDiskFactory();

	public IEnumerator<PhysicalDiskGpt> GetEnumerator()
	{
		for (ulong i = 0;; ++i)
		{
			var physicalDiskHandle = CreateFileW(@$"\\.\PhysicalDrive{i}",
				DesiredAccess.GenericRead, FileShare.Read, ref NullSecurityAttributes, FileMode.Open,
				FileFlagsAndAttributes.NoBuffering | FileFlagsAndAttributes.WriteThrough, IntPtr.Zero
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

			yield return disk;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}