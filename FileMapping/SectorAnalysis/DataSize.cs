using System.Globalization;
using System.Numerics;

namespace FileMapping.SectorAnalysis;

public readonly struct DataSize(ulong actual)
	: IFormattable, IComparisonOperators<DataSize, DataSize, bool>, IEquatable<DataSize>
{
	internal readonly ulong Actual = actual;
	public override string ToString() => ToString(null, CultureInfo.CurrentCulture);

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		return format switch
		{
			null => AutoMatch(),
			"B" => Actual + " B",
			[ _, 'i', 'B' ] =>
				$"{(Actual / GetUnitScale(format[0], true)).ToString(formatProvider)} {format}",
			[ _, 'B' ] =>
				$"{(Actual / GetUnitScale(format[0], false)).ToString(formatProvider)} {format}",
			_ => throw new Exception()
		};
	}

	public static implicit operator ulong(DataSize d) => d.Actual;

	private static ulong GetUnitScale(char magnitude, bool isBi)
	{
		var units = isBi
			? new Dictionary<char, ulong>
			{
				{ 'K', (ulong)BinaryUnits.KiB },
				{ 'M', (ulong)BinaryUnits.MiB },
				{ 'G', (ulong)BinaryUnits.GiB },
				{ 'T', (ulong)BinaryUnits.TiB },
				{ 'P', (ulong)BinaryUnits.PiB }
			}
			: new Dictionary<char, ulong>
			{
				{ 'K', (ulong)DecimalUnits.KB },
				{ 'M', (ulong)DecimalUnits.MB },
				{ 'G', (ulong)DecimalUnits.GB },
				{ 'T', (ulong)DecimalUnits.TB },
				{ 'P', (ulong)DecimalUnits.PB }
			};

		return units.TryGetValue(magnitude, out var unit) ? unit : throw new Exception();
	}

	// ReSharper disable InconsistentNaming
	private enum BinaryUnits : ulong
	{
		KiB = 1UL << 10,
		MiB = KiB << 10,
		GiB = MiB << 10,
		TiB = GiB << 10,
		PiB = TiB << 10
	}

	private enum DecimalUnits : ulong
	{
		KB = 1_000,
		MB = 1_000_000,
		GB = 1_000_000_000,
		TB = 1_000_000_000_000,
		PB = 1_000_000_000_000_000
	}
	// ReSharper restore InconsistentNaming

	private string AutoMatch()
	{
		if (Actual % (ulong)BinaryUnits.KiB == 0)
			return Actual switch
			{
				< (ulong)BinaryUnits.KiB => $"{Actual} B",
				>= (ulong)BinaryUnits.KiB and < (ulong)BinaryUnits.MiB => $"{Actual / (ulong)BinaryUnits.KiB} KiB",
				>= (ulong)BinaryUnits.MiB and < (ulong)BinaryUnits.GiB => $"{Actual / (ulong)BinaryUnits.MiB} MiB",
				>= (ulong)BinaryUnits.GiB and < (ulong)BinaryUnits.TiB => $"{Actual / (ulong)BinaryUnits.GiB} GiB",
				>= (ulong)BinaryUnits.TiB and < (ulong)BinaryUnits.PiB => $"{Actual / (ulong)BinaryUnits.TiB} TiB",
				>= (ulong)BinaryUnits.PiB => $"{Actual / (ulong)BinaryUnits.PiB} PiB"
			};
		return Actual switch
		{
			< (ulong)DecimalUnits.KB => $"{Actual} B",
			>= (ulong)DecimalUnits.KB and < (ulong)DecimalUnits.MB => $"{Actual / (ulong)DecimalUnits.KB} KB",
			>= (ulong)DecimalUnits.MB and < (ulong)DecimalUnits.GB => $"{Actual / (ulong)DecimalUnits.MB} MB",
			>= (ulong)DecimalUnits.GB and < (ulong)DecimalUnits.TB => $"{Actual / (ulong)DecimalUnits.GB} GB",
			>= (ulong)DecimalUnits.TB and < (ulong)DecimalUnits.PB => $"{Actual / (ulong)DecimalUnits.TB} TB",
			>= (ulong)DecimalUnits.PB => $"{Actual / (ulong)DecimalUnits.PB} PiB"
		};
	}

	public bool Equals(DataSize other) => Actual == other.Actual;

	public int CompareTo(DataSize other) => Actual.CompareTo(other.Actual);

	public override bool Equals(object? obj) =>
		obj switch
		{
			null => false,
			DataSize d => Equals(d),
			_ => false
		};

	public override int GetHashCode() => Actual.GetHashCode();

	public static bool operator ==(DataSize left, DataSize right) => left.Equals(right);

	public static bool operator !=(DataSize left, DataSize right) => !(left == right);

	public static bool operator <(DataSize left, DataSize right) => left.CompareTo(right) < 0;

	public static bool operator <=(DataSize left, DataSize right) => left.CompareTo(right) <= 0;

	public static bool operator >(DataSize left, DataSize right) => left.CompareTo(right) > 0;

	public static bool operator >=(DataSize left, DataSize right) => left.CompareTo(right) >= 0;
}