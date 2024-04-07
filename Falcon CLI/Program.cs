/*using FileMapping.RawDiskProber;

using var deviceHandle = DiskSectorReader.GetPhysicalDiskHandle(0);
if (deviceHandle is null)
{
	Console.WriteLine("物理磁盘句柄无效");
	return;
}
Console.WriteLine("设备句柄有效");

if (DiskSectorReader.ReadSect(deviceHandle,Parser))
{
	Console.WriteLine("正常结束");
	return;
}
Console.WriteLine("不正常");
return;

static bool Parser(byte[] buffer)
{
	Console.WriteLine(buffer[510]);
	return true;
}*/

using FileMapping.SectorAnalysis;
using FileMapping.SectorAnalysis.MFT;
using FileMapping.SectorAnalysis.PhysicalDisk;

foreach (var i in PhysicalDiskFactory.Instance)
{
	Console.WriteLine($"""
	                   {i.Id}
	                   {i.DiskSize}
	                   {i.Geometry.MediaType}
	                   {i.Geometry.BytesPerSector}
	                   {i.Geometry.Cylinders}
	                   {i.Geometry.TracksPerCylinder}
	                   {i.Geometry.SectorsPerTrack}
	                   {i.LayoutInformation.DiskId}
	                   {i.LayoutInformation.StartingUsableOffset}
	                   {i.LayoutInformation.UsableLength}
	                   """);
	foreach (var ii in i.PartitionEntries)
	{
		Console.WriteLine($"offset: {ii.StartingOffset} describe: {ii.Name}");

	}

	foreach (var v in new MftStreamFactory(i))
	{
		v.ParseBoot();
	}
	
}