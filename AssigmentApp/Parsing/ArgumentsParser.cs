using AssigmentApp.Types;
using FluentResults;

namespace AssigmentApp.Parsing;

public static class ArgumentsParser
{
    public static Result<ProgramOptions> Parse(string[] args)
    {
        if (args.Length == 0)
            return Result.Fail("No arguments provided.");

        if (args.Length % 2 != 0)
            return Result.Fail("Arguments must be provided as name/value pairs.");

        string? hotelsPath = null;
        string? bookingsPath = null;

        for (var i = 0; i < args.Length; i += 2)
        {
            var name = args[i];
            if (i + 1 >= args.Length)
                return Result.Fail($"Argument '{name}' is missing a value.");

            var value = args[i + 1];
            switch (name)
            {
                case ProgramConstants.HotelsArgName:
                    if (hotelsPath is not null)
                        return Result.Fail("Duplicate hotels argument.");
                    hotelsPath = value;
                    break;
                case ProgramConstants.BookingsArgName:
                    if (bookingsPath is not null)
                        return Result.Fail("Duplicate bookings argument.");
                    bookingsPath = value;
                    break;
                default:
                    return Result.Fail($"Unknown argument '{name}'.");
            }
        }

        if (string.IsNullOrWhiteSpace(hotelsPath))
            return Result.Fail("Hotels file path was not provided.");

        if (string.IsNullOrWhiteSpace(bookingsPath))
            return Result.Fail("Bookings file path was not provided.");

        return Result.Ok(new ProgramOptions(hotelsPath!, bookingsPath!));
    }
}
