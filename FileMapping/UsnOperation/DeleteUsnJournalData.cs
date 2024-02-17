using System.Runtime.InteropServices;

namespace FileMapping.UsnOperation;

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct DeleteUsnJournalData
{
	internal ulong UsnJournalId { get; init; }
	internal UsnDeleteFlags DeleteFlags { get; init; }
}