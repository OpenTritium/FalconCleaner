using FileMapping.RawDiskProber;

using var deviceHandle = RawDiskUtils.GetPhysicalDiskHandle(1);
if (deviceHandle is null)
{
	Console.WriteLine("物理磁盘句柄无效");
	return;
}
Console.WriteLine("设备句柄有效");
if (!RawDiskUtils.ShiftPointerForward(deviceHandle, 0, out var newOne))
{
	Console.WriteLine("移动句柄失败");
	return;
}
Console.WriteLine("移动句柄成功");

if (RawDiskUtils.ExploitBytes(deviceHandle, 512, Parser))
{
	Console.WriteLine("正常结束");
	return;
}
Console.WriteLine("不正常");
return;

static bool Parser(byte[] buffer, uint bytesCount)
{
	Console.WriteLine(buffer[510]);
	return true;
}