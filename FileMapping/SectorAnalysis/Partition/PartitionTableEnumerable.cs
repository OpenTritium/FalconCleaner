using System.Collections;
using FileMapping.PInvoke.DriveLayout;

namespace FileMapping.SectorAnalysis.Partition;

internal unsafe class PartitionTableEnumerable(
    DriveLayoutInformationEx* driveLayoutInformationExPointer) : IEnumerable<PartitionInformationEx>,
    IEnumerator<PartitionInformationEx>
{
    private readonly uint _partitionCount = driveLayoutInformationExPointer->PartitionCount;
    private uint _rowIndicator;
    private PartitionInformationEx* _cursor = &driveLayoutInformationExPointer->PartitionEntry;

    public IEnumerator<PartitionInformationEx> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool MoveNext()
    {
        ++_rowIndicator;
        if (_rowIndicator > _partitionCount) return false;
        if (_rowIndicator != 1) ++_cursor;
        return true;
    }

    public void Reset() { }

    public PartitionInformationEx Current => *_cursor;

    object IEnumerator.Current => Current;

    public void Dispose() { }
}