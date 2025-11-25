using AssigmentApp.Types.Commands;
using FluentResults;

namespace AssigmentApp.Parsing;

public static class CommandParser
{
    public static Result<T> Parse<T>(string input) where T : struct
    {
        if (string.IsNullOrWhiteSpace(input))
            return Result.Fail("Command is empty.");

        input = input.Trim();

        if (!TryExtract(input, out var commandName, out var argumentsText))
            return Result.Fail("Command format is invalid. Expected Command(arg1, arg2, ...).");

        var expectedName = typeof(T) == typeof(AvailabilityCommandArguments)
            ? nameof(CommandType.Availability)
            : typeof(T) == typeof(SearchCommandArguments)
                ? nameof(CommandType.Search)
                : null;

        if (expectedName is null)
            return Result.Fail($"Unsupported command type '{typeof(T).Name}'.");

        if (!commandName.Equals(expectedName, StringComparison.OrdinalIgnoreCase))
            return Result.Fail($"Unknown command '{commandName}'.");

        return ParseArguments<T>(argumentsText);
    }

    private static bool TryExtract(string input, out string command, out string args)
    {
        command = string.Empty;
        args = string.Empty;

        var open = input.IndexOf('(');
        var close = input.LastIndexOf(')');

        if (open <= 0 || close <= open || close != input.Length - 1)
            return false;

        command = input[..open].Trim();
        args = input[(open + 1)..close];
        return !string.IsNullOrWhiteSpace(command);
    }

    private static Result<T> ParseArguments<T>(string arguments) where T : struct
    {
        var parts = arguments.Split(',', StringSplitOptions.TrimEntries);

        if (typeof(T) == typeof(AvailabilityCommandArguments))
        {
            if (parts.Length != 3)
                return Result.Fail("Availability command expects 3 arguments: HotelId, DateOrRange, RoomType.");

            var hotelId = parts[0];
            var datePart = parts[1];
            var roomType = parts[2];

            if (string.IsNullOrWhiteSpace(hotelId) || string.IsNullOrWhiteSpace(roomType))
                return Result.Fail("HotelId and RoomType must be provided.");

            var dateResult = BookingDateParser.ParseCommandDateOrRange(datePart);
            if (dateResult.IsFailed)
                return Result.Fail(dateResult.Errors);

            var result = new AvailabilityCommandArguments(hotelId, dateResult.Value, roomType);
            return Result.Ok((T)(object)result);
        }

        if (typeof(T) == typeof(SearchCommandArguments))
        {
            if (parts.Length != 3)
                return Result.Fail("Search command expects 3 arguments: HotelId, DaysToSearch, RoomType.");

            var hotelId = parts[0];
            var daysPart = parts[1];
            var roomType = parts[2];

            if (string.IsNullOrWhiteSpace(hotelId) || string.IsNullOrWhiteSpace(roomType))
                return Result.Fail("HotelId and RoomType must be provided.");

            if (!int.TryParse(daysPart, out var days))
                return Result.Fail("DaysToSearch must be an integer.");

            if (days < 0)
                return Result.Fail("DaysToSearch cannot be negative.");

            var result = new SearchCommandArguments(hotelId, days, roomType);
            return Result.Ok((T)(object)result);
        }

        return Result.Fail($"Unsupported command type '{typeof(T).Name}'.");
    }
}
