using FileMapping.UsnOperation;

var (volumeName, fileSystemName, serialNumber, fileSystemFlags) = UsnOperationUtils.GetVolumeInfo('C');
Console.WriteLine($"{serialNumber} {volumeName} {fileSystemName} {fileSystemFlags}");
var hDevice = UsnOperationUtils.CreateFile('C');
if (hDevice is null)
{
	Console.WriteLine("句柄无效");
	return;
}

if (UsnOperationUtils.CreateUsnJournal(hDevice)) Console.WriteLine("创建成功");

if (UsnOperationUtils.QueryUsnJournal(hDevice, out var usnJournalData))
{
	Console.WriteLine("查询成功");
	foreach (var (currentFileRef, parentFileRef, fileName) in
	         new UsnOperationUtils.UsnRecordEnumerable(hDevice, usnJournalData.NextUsn))
		Console.WriteLine($"{currentFileRef} {parentFileRef} {fileName}");
	if (UsnOperationUtils.DeleteUsnJournal(hDevice, usnJournalData)) Console.WriteLine("删除日志成功");
}