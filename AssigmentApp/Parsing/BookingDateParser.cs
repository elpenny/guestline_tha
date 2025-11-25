using System.Globalization;
using AssigmentApp.Types.Commands;
using FluentResults;

namespace AssigmentApp.Parsing;

public static class BookingDateParser
{
    private const string DateFormat = "yyyyMMdd";
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    /// <summary>
    /// Low-level literal date parsing. Used by JSON converters for arrival/departure.
    /// </summary>
    public static DateOnly ParseDate(string s) =>
        DateOnly.ParseExact(s, DateFormat, Culture);

    /// <summary>
    /// Safe literal date parsing with validation.
    /// </summary>
    public static Result<DateOnly> ParseDateResult(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return Result.Fail("Date value is empty.");

        if (DateOnly.TryParseExact(
                s,
                DateFormat,
                Culture,
                DateTimeStyles.None,
                out var date))
        {
            return Result.Ok(date);
        }

        return Result.Fail($"Invalid date '{s}'. Expected format: {DateFormat}.");
    }

    /// <summary>
    /// Parse arrival/departure dates from bookings.json as a literal range.
    /// Interprets them as [arrival, departure) (arrival inclusive, departure exclusive).
    /// </summary>
    public static Result<(DateOnly Arrival, DateOnly Departure)> ParseBookingDates(
        string arrival,
        string departure)
    {
        var arrivalResult = ParseDateResult(arrival);
        if (arrivalResult.IsFailed)
            return Result.Fail(arrivalResult.Errors);

        var departureResult = ParseDateResult(departure);
        if (departureResult.IsFailed)
            return Result.Fail(departureResult.Errors);

        var a = arrivalResult.Value;
        var d = departureResult.Value;

        if (d < a)
            return Result.Fail(
                $"Departure date '{d:yyyyMMdd}' is before arrival '{a:yyyyMMdd}'.");

        if (d == a)
            return Result.Fail(
                $"Departure date '{d:yyyyMMdd}' is equal to arrival '{a:yyyyMMdd}'. " +
                "Bookings must cover at least one night.");

        return Result.Ok((a, d));
    }

    /// <summary>
    /// Parse a literal date range expressed as "yyyyMMdd-yyyyMMdd".
    /// Does NOT apply hotel semantics; just returns [from, to] as typed.
    /// </summary>
    public static Result<(DateOnly From, DateOnly To)> ParseDateRangeLiteral(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return Result.Fail("Date range value is empty.");

        var parts = s.Split('-', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
            return Result.Fail(
                $"Invalid date range '{s}'. Expected format: {DateFormat}-{DateFormat}.");

        var fromResult = ParseDateResult(parts[0]);
        if (fromResult.IsFailed)
            return Result.Fail(fromResult.Errors);

        var toResult = ParseDateResult(parts[1]);
        if (toResult.IsFailed)
            return Result.Fail(toResult.Errors);

        var from = fromResult.Value;
        var to = toResult.Value;

        if (to < from)
            return Result.Fail(
                $"End date '{to:yyyyMMdd}' is before start date '{from:yyyyMMdd}'.");

        return Result.Ok((from, to));
    }

    /// <summary>
    /// Parse command date argument for Availability/Search:
    /// - "yyyyMMdd"             → one-night stay [date, date+1)
    /// - "yyyyMMdd-yyyyMMdd"    → [start, end) with end after start
    ///
    /// Use this for console commands so single dates behave like real one-night stays.
    /// </summary>
    public static Result<DateRange> ParseCommandDateOrRange(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return Result.Fail("Date value is empty.");

        if (s.Contains('-'))
        {
            var literalRangeResult = ParseDateRangeLiteral(s);
            if (literalRangeResult.IsFailed)
                return Result.Fail(literalRangeResult.Errors);

            var (from, to) = literalRangeResult.Value;

            if (to == from)
            {
                return Result.Fail(
                    $"Date range '{from:yyyyMMdd}-{to:yyyyMMdd}' " +
                    "must span at least one night (end must be after start).");
            }

            return Result.Ok(new DateRange(from, to));
        }

        var dateResult = ParseDateResult(s);
        if (dateResult.IsFailed)
            return Result.Fail(dateResult.Errors);

        var date = dateResult.Value;
        return Result.Ok(new DateRange(date, date.AddDays(1)));
    }
}
