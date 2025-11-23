using System.Globalization;
using AssigmentApp.Types;

namespace AssigmentApp.Parsing;

public static class BookingDateParser
{
    private const string DateFormat = "yyyyMMdd";

    public static DateOnly ParseDate(string s) =>
        DateOnly.ParseExact(s, DateFormat, CultureInfo.InvariantCulture);

    public static bool TryParseDate(string s, out DateOnly date) =>
        DateOnly.TryParseExact(s, DateFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out date);

    public static bool TryParseDateRange(string s, out DateOnly from, out DateOnly to)
    {
        var parts = s.Split('-', 2, StringSplitOptions.TrimEntries);
        if (parts.Length == 2 && TryParseDate(parts[0], out from) && TryParseDate(parts[1], out to)) return true;
        from = default;
        to = default;
        
        return false;
    }
    
    public static bool TryParseSingleOrRange(string s, out DateOnly from, out DateOnly to)
    {
        if (TryParseDateRange(s, out from, out to))
            return true;

        if (TryParseDate(s, out from))
        {
            to = from;
            return true;
        }

        to = default;
        return false;
    }
    
    public static bool TryParseDateRange(string s, out DateRange range)
    {
        if (TryParseSingleOrRange(s, out var from, out var to))
        {
            range = new DateRange(from, to);
            return true;
        }

        range = default;
        return false;
    }
}