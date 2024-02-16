using System.Runtime.InteropServices;
using FileMapping.UsnOperation;
using FileMapping;

try
{
    const uint maxBufferLen = 261;
    var volumeNameBuffer = new char[maxBufferLen];
    var fileSystemNameBuffer = new char[maxBufferLen];

    var result = Win32Api.GetVolumeInformationW(
        @"C:\",
        volumeNameBuffer,
        maxBufferLen,
        out var serialNumber,
        out _,
        out var fileSystemFlags,
        fileSystemNameBuffer,
        maxBufferLen);

    if (result)
    {
        Console.WriteLine("Volume Name: " + new string(volumeNameBuffer));
        Console.WriteLine("Volume Serial Number: " + serialNumber);
        Console.WriteLine("File System: " + new string(fileSystemNameBuffer));
        var flags = (FileSystemFlags)fileSystemFlags;
        foreach (var flag in Enum.GetValues<FileSystemFlags>())
        {
            if ((flags & flag) == flag)
            {
                Console.WriteLine(flag);
            }
        }
    }
    else
    {
        Console.WriteLine("Unable to retrieve volume information.");
        Console.ReadKey();
        return;
    }


    var volHandle = Win32Api.CreateFileW(
        @"\\.\C:",
        DesiredAccess.ReadWrite,
        FileShare.ReadWrite,
        IntPtr.Zero,
        FileMode.Open,
        FileAttributes.ReadOnly,
        IntPtr.Zero
    );

    if (volHandle.IsInvalid)
    {
        Console.WriteLine("句柄坏的，考虑给一下管理员权限，按下回车程序退出");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("拿到句柄了哦耶");

    var requestCreateUsnJournal =  new CreateUsnJournalData
    {
        AllocationDelta = 0,
        MaximumSize = 0
    };
    unsafe
    {
        var createUsnJournalResult = Win32Api.DeviceIoControl(
            volHandle,
            FileSystemControlCode.CreateUsnJournal,
            new IntPtr(&requestCreateUsnJournal),
            (uint)sizeof(CreateUsnJournalData),
            IntPtr.Zero,
            0,
            out _,
            IntPtr.Zero
        );

        Console.WriteLine(createUsnJournalResult ? "创建USN成功" : "创建USN失败");
    }



    UsnJournalDataV2 responseJournalData = default;
    unsafe
    {
        var queryUsnJournalResult = Win32Api.DeviceIoControl(
            volHandle,
            FileSystemControlCode.QueryUsnJournal,
            IntPtr.Zero,
            0,
            new IntPtr(&responseJournalData),
            (uint)sizeof(UsnJournalDataV2),
            out _, // 一般是 80 字节
            IntPtr.Zero
        );
        if (queryUsnJournalResult)
        {
            Console.WriteLine("创建查询日志");
        }
        else
        {
            Console.WriteLine("创建失败");
            Console.ReadKey();
            return;
        }
    }

    var requestEnumData = new MftEnumDataV1
    {
        HighUsn = responseJournalData.NextUsn,
        LowUsn = 0,
        MaxMajorVersion = responseJournalData.MaxSupportedMajorVersion,
        MinMajorVersion = 3,
        StartFileReferenceNumber = 0 //这里经测试发现，如果用FirstUsn有时候不正确，导致获取到不完整的数据，还是直接写0好. 
    };

    const uint bufferSize = 0xFFFF_FFF;
//如果 缓冲区不够 GetLastError 返回 ERROR_INSUFFICIENT_BUFFER，并且 BytesReturned 为0
// 如果只够写一部分 GetLastError 则返回 ERROR_MORE_DATA ，并且 lpBytesReturned 会回答已写入的量
// 记得缓存复位
    unsafe
    {

        var pBuffer = (IntPtr)NativeMemory.Alloc(bufferSize);
        while (
            Win32Api.DeviceIoControl(
                volHandle,
                FileSystemControlCode.EnumUsnData,
                new IntPtr(&requestEnumData),
                (uint)sizeof(MftEnumDataV1),
                pBuffer,
                bufferSize,
                out var remainingBytes,
                IntPtr.Zero
            ))
        {
            // 缓冲区前 8 字节是下一条 USN，然后才是记录
            const byte nextUsnOffset = 8;
            remainingBytes -= nextUsnOffset;
            var pCurrentRecord = IntPtr.Add(pBuffer, nextUsnOffset);
            requestEnumData.StartFileReferenceNumber = *(ulong*)pBuffer;
            while (remainingBytes != 0)
            {
                var pHeader = (UsnRecordCommonHeader*)pCurrentRecord;
                switch (pHeader->MajorVersion, pHeader->MinorVersion)
                {
                    case (3, 0):
                        var pUsnRecordV3 = (UsnRecordV3*)pCurrentRecord;
                        var fileNameV3 = Marshal.PtrToStringUni(
                            IntPtr.Add(pCurrentRecord, pUsnRecordV3->FileNameOffset),
                            pUsnRecordV3->FileNameLength / sizeof(char));
                        Console.WriteLine(fileNameV3);
                        var llc = new UInt128(lower: pUsnRecordV3->FileReferenceNumber[0],
                            upper: pUsnRecordV3->FileReferenceNumber[1]);
                        Console.WriteLine($"current : {llc}");
                        var llp = new UInt128(lower: pUsnRecordV3->ParentFileReferenceNumber[0],
                            upper: pUsnRecordV3->ParentFileReferenceNumber[1]);
                        Console.WriteLine($"parent: {llp}");
                        break;
                    case (4, 0):
                        // 一群 V4 后面会跟随一个 V3
                        //当文件系统发生一系列相关的更改时，例如文件被重命名，Windows可能会在USN日志中添加一组V4记录，然后跟随一个V3记录。V4记录用于跟踪每个单独的更改，
                        //而V3记录则提供了一个总结，包括文件的新名称和旧名称。
                        //这样做的原因可能是为了提高效率和性能。V4记录比V3记录更小，
                        //因此，当文件系统发生大量更改时，使用V4记录可以节省磁盘空间和I/O资源。然后，通过在一组V4记录后面添加一个V3记录，Windows可以提供一个总结，使得应用程序可以更容易地理解和处理这些更改。
                        var pUsnRecordV4 = (UsnRecordV4*)pCurrentRecord;
                        var llc4 = new UInt128(lower: pUsnRecordV4->FileReferenceNumber[0],
                            upper: pUsnRecordV4->FileReferenceNumber[1]);
                        Console.WriteLine($"current : {llc4}");
                        var llp4 = new UInt128(lower: pUsnRecordV4->ParentFileReferenceNumber[0],
                            upper: pUsnRecordV4->ParentFileReferenceNumber[1]);
                        Console.WriteLine($"parent: {llp4}");
                        Console.WriteLine("没有名称");
                        break;
                    default:
                        Console.WriteLine("不支持的版本号");
                        break;
                }

                remainingBytes -= pHeader->RecordLength;
                pCurrentRecord = IntPtr.Add(pCurrentRecord, (int)pHeader->RecordLength);
            }
        }

        NativeMemory.Free(pBuffer.ToPointer());
    }

    var requestDeleteUsnJournal = new DeleteUsnJournalData
    {
        DeleteFlags = UsnDeleteFlags.DeleteAndNotify,
        UsnJournalId = responseJournalData.UsnJournalId
    };

    unsafe
    {
        var deleteUsnJournalResult = Win32Api.DeviceIoControl(
            volHandle,
            FileSystemControlCode.DeleteUsnJournal,
            new IntPtr(&requestCreateUsnJournal),
            (uint)sizeof(DeleteUsnJournalData),
            IntPtr.Zero,
            0,
            out _,
            IntPtr.Zero
        );
        Console.WriteLine(deleteUsnJournalResult ? "清除日志成功" : "清除日志失败");
    }

    Console.ReadKey();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Console.ReadKey();
}