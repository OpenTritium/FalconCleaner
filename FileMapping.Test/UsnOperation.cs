using FileMapping.UsnOperation;
using System.Diagnostics;

namespace FileMapping.Test;

[TestClass]
public class UsnOperation
{
	[DataRow('A')]
	[DataRow('B')]
	[TestMethod]
	public void InvalidVolumeHandle(char diskLetter)
	{
		var hDevice = UsnUtils.GetVolumeHandle(diskLetter);
		Assert.IsNull(hDevice, "使用非法盘符获取的句柄竟然非空");
	}
	[DataRow('C')]
	[TestMethod]
	public void CreateReadAndDeleteUsnJournal(char diskLetter)
	{
		var (volumeName, fileSystemName, serialNumber, fileSystemFlags) = UsnUtils.GetVolumeInfo(diskLetter);
		Trace.WriteLine($"{serialNumber} {volumeName} {fileSystemName} {fileSystemFlags}");
		var hDevice = UsnUtils.GetVolumeHandle(diskLetter);
		Assert.IsNotNull(hDevice, "创建句柄失败");
		Assert.IsTrue(UsnUtils.CreateUsnJournal(hDevice), "USN 日志创建失败");
		Assert.IsTrue(UsnUtils.QueryUsnJournal(hDevice, out var usnJournalData), "查询 USN 日志失败");
		foreach (var (currentFileRef, parentFileRef, fileName) in
		         new UsnUtils.UsnRecordEnumerable(hDevice, usnJournalData.NextUsn))
			Trace.WriteLine($"{currentFileRef} {parentFileRef} {fileName}");
		Assert.IsTrue(UsnUtils.DeleteUsnJournal(hDevice, usnJournalData), "删除 USN 日志失败");
	}
}