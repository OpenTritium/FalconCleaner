using System.Runtime.InteropServices;
using FileMapping.UsnOperation;

var result = UsnOperation.GetVolumeInfo(@"C:\");
Console.WriteLine($"{result.serialNumber} {result.volumeName} {result.fileSystemName} {result.fileSytemFlags}");
var hDevice = UsnOperation.CreateFile(@"C:\");
if (hDevice is null)
{
	Console.WriteLine("句柄无效");
	return;
}
if(UsnOperation.CreateUsnJournal(hDevice)){
	Console.WriteLine("创建成功");
}

if (UsnOperation.QueryUsnJournal(hDevice, out var usnJournalData))
{
	Console.WriteLine("查询成功");
	UsnOperation.EnumUsnData(hDevice, usnJournalData);
	if (UsnOperation.DeleteUsnJournal(hDevice, usnJournalData))
	{
		Console.WriteLine("删除日志成功");
	}
}