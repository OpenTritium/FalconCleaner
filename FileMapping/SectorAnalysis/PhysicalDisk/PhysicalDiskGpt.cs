using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FileMapping.PInvoke;
using FileMapping.PInvoke.DiskGeometry;
using FileMapping.PInvoke.DriveLayout;
using FileMapping.SectorAnalysis.Partition;
using Microsoft.Win32.SafeHandles;
using static FileMapping.PInvoke.Win32Api;

namespace FileMapping.SectorAnalysis.PhysicalDisk;

/// <summary>
/// 只保证 GPT 硬盘的顺利构建，遇到 MBR 硬盘会抛出异常
/// 持有物理磁盘句柄，及时更新物理磁盘参数
/// </summary>
// todo 响应磁盘变化事件
internal sealed class PhysicalDiskGpt : IDisposable
{
    internal readonly ulong Id;
    internal readonly SafeFileHandle Handle; // 持有句柄是为了更新信息
    internal readonly DiskGeometry Geometry;
    internal readonly DataSize DiskSize;
    internal readonly uint PartitionCount;
    internal readonly DriveLayoutInformationGpt LayoutInformation;
    internal readonly PartitionInformationGptRecord[] PartitionEntries;

    internal unsafe PhysicalDiskGpt(ulong id, SafeFileHandle handle)
    {
        Id = id;
        Handle = handle;

        // 此大小是基于 6 个分区条目得来的，除开 ESP 和 MSR，我觉得大多数人 4 逻辑驱动器是上限，当然 DiskGenius 也喜欢给用户分四个
        var presetSize = (uint)(sizeof(DriveLayoutInformationEx) + 5 * sizeof(PartitionInformationEx));
        var maxSize = (uint)(sizeof(DriveLayoutInformationEx) + 27 * sizeof(PartitionInformationEx));
        var bufferPointer = NativeMemory.Alloc(presetSize);

        while (true)
        {
            if (presetSize >= maxSize) throw new Win32Exception();
            if (TryGetDriveLayout()) break;
        }

        //todo 异常 仅仅不加载 非 GPt 硬盘
        var driveLayoutInformationExPointer = (DriveLayoutInformationEx*)bufferPointer;
        try
        {
            if (driveLayoutInformationExPointer->PartitionStyle != PartitionTableStyle.GPT)
                throw new Exception();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception.Message);
            NativeMemory.Free(bufferPointer);
        }

        PartitionCount = driveLayoutInformationExPointer->PartitionCount;
        LayoutInformation = driveLayoutInformationExPointer->DummyUnionName.Gpt;

        PartitionEntries = new PartitionTableEnumerable(driveLayoutInformationExPointer).Select(Bridge).ToArray();
        DeviceIoControl(handle, IoControlCodes.GetDriveGeometryEx, IntPtr.Zero, 0, new IntPtr(bufferPointer),
            presetSize, out _,IntPtr.Zero);
        var diskGeometryExPointer = (DiskGeometryEx*)bufferPointer;
        DiskSize = new DataSize((ulong)diskGeometryExPointer->DiskSize);
        Geometry = diskGeometryExPointer->geometry;
        NativeMemory.Free(bufferPointer);
        return;

        PartitionInformationGptRecord Bridge(PartitionInformationEx info)
        {
            var name = Marshal.PtrToStringUni((nint)info.DummyUnionName.Gpt.Name, 36);
            return new PartitionInformationGptRecord
            {
                Attributes = info.DummyUnionName.Gpt.Attributes,
                IsServicePartition = info.IsServicePartition,
                Name = name,
                PartitionId = info.DummyUnionName.Gpt.PartitionId,
                PartitionNumber = info.PartitionNumber,
                PartitionType = info.DummyUnionName.Gpt.PartitionType,
                PartitionLength = info.PartitionLength,
                RewritePartition = info.RewritePartition,
                StartingOffset = info.StartingOffset
            };
        }

        bool TryGetDriveLayout()
        {
            try
            {
                return DeviceIoControl(Handle, IoControlCodes.GetDriveLayoutEx, IntPtr.Zero, 0,
                    new IntPtr(bufferPointer), presetSize, out _, IntPtr.Zero)
                    ? true
                    : throw new Win32Exception();
            }
            catch (Win32Exception exception)
            {
                Debug.WriteLine(exception.Message);
                presetSize += (uint)sizeof(PartitionInformationEx);
                bufferPointer = NativeMemory.Realloc(bufferPointer, presetSize);
                return false;
            }
        }
    }

    public void Dispose() => Handle.Dispose();
}