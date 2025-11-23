using System.Globalization;

namespace AssigmentApp.Types.Commands;

public readonly record struct SearchCommandArguments(string HotelName, int DaysToSearch, string RoomType);

public sealed record SearchResult(IReadOnlyList<SearchResultEntry> Entries)
{
    public override string ToString()
    {
        var parts = Entries.Select(e =>
            $"({FormatRange(e.Range)}, {e.RoomsAvailable})");
        return string.Join(", ", parts);
    }

    private static string FormatRange(DateRange range) =>
        range.IsSingleDay
            ? range.Start.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
            : $"{range.Start:yyyyMMdd}-{range.End:yyyyMMdd}";
}

public readonly record struct SearchResultEntry(
    DateRange Range,
    int RoomsAvailable
);
