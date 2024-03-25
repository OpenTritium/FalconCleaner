using System.Runtime.InteropServices;

namespace FileMapping.PInvoke.Usn;

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct DeleteUsnJournalData
{
	internal ulong UsnJournalId { get; init; }
	internal UsnDeleteFlags DeleteFlags { get; init; }
}