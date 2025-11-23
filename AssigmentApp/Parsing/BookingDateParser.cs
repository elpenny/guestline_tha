using System.Globalization;
using AssigmentApp.Types.Commands;
using FluentResults;

namespace AssigmentApp.Parsing;

public static class BookingDateParser
{
    private const string DateFormat = "yyyyMMdd";

    public static DateOnly ParseDate(string s) =>
        DateOnly.ParseExact(s, DateFormat, CultureInfo.InvariantCulture);
    
    public static Result<DateOnly> ParseDateResult(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return Result.Fail("Date value is empty.");

        if (DateOnly.TryParseExact(
                s,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            return Result.Ok(date);
        }

        return Result.Fail($"Invalid date '{s}'. Expected format: {DateFormat}.");
    }
    
    public static Result<(DateOnly From, DateOnly To)> ParseDateRange(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return Result.Fail("Date range value is empty.");

        var parts = s.Split('-', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            return Result.Fail($"Invalid date range '{s}'. Expected format: {DateFormat}-{DateFormat}.");

        var fromResult = ParseDateResult(parts[0]);
        if (fromResult.IsFailed)
            return Result.Fail(fromResult.Errors);

        var toResult = ParseDateResult(parts[1]);
        if (toResult.IsFailed)
            return Result.Fail(toResult.Errors);

        var from = fromResult.Value;
        var to = toResult.Value;

        if (to < from)
            return Result.Fail($"End date '{to:yyyyMMdd}' is before start date '{from:yyyyMMdd}'.");

        return Result.Ok((from, to));
    }
    
    public static Result<DateRange> ParseSingleOrRange(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return Result.Fail("Date value is empty.");
        
        if (s.Contains('-'))
        {
            var rangeResult = ParseDateRange(s);
            if (rangeResult.IsFailed)
                return Result.Fail(rangeResult.Errors);

            var (from, to) = rangeResult.Value;
            return Result.Ok(new DateRange(from, to));
        }
        
        var dateResult = ParseDateResult(s);
        if (dateResult.IsFailed)
            return Result.Fail(dateResult.Errors);

        var date = dateResult.Value;
        return Result.Ok(new DateRange(date, date));
    }
}
