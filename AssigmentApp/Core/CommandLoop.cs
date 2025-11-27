using AssigmentApp.Types;
using AssigmentApp.Types.Commands;
using AssigmentApp.Parsing;

namespace AssigmentApp.Core;

public static class CommandLoop
{
    public static void Run(TextReader reader, TextWriter writer, ProgramState state)
    {
        writer.WriteLine(ProgramConstants.HelpMessage);

        while (true)
        {
            var line = reader.ReadLine();
            if (line is null || string.IsNullOrWhiteSpace(line))
                break;

            var commandName = ExtractCommandName(line);
            if (commandName is null)
            {
                writer.WriteLine("Invalid command format. Expected Command(arg1, arg2, ...).");
            }
            else if (commandName.Equals(nameof(CommandType.Availability), StringComparison.OrdinalIgnoreCase))
            {
                var parseResult = CommandParser.Parse<AvailabilityCommandArguments>(line);
                if (parseResult.IsFailed)
                {
                    WriteErrors(writer, parseResult.Errors);
                }
                else
                {
                    var result = CommandsHandler.HandleAvailability(state, parseResult.Value);
                    if (result.IsFailed)
                        WriteErrors(writer, result.Errors);
                    else
                        writer.WriteLine(result.Value.RoomsAvailable);
                }
            }
            else if (commandName.Equals(nameof(CommandType.Search), StringComparison.OrdinalIgnoreCase))
            {
                var parseResult = CommandParser.Parse<SearchCommandArguments>(line);
                if (parseResult.IsFailed)
                {
                    WriteErrors(writer, parseResult.Errors);
                }
                else
                {
                    var result = CommandsHandler.HandleSearch(state, parseResult.Value);
                    if (result.IsFailed)
                        WriteErrors(writer, result.Errors);
                    else
                        writer.WriteLine(result.Value.ToString());
                }
            }
            else
            {
                writer.WriteLine($"Unknown command '{commandName}'.");
            }

            writer.WriteLine(ProgramConstants.HelpMessage);
        }
    }

    private static string? ExtractCommandName(string input)
    {
        var trimmed = input.Trim();
        var idx = trimmed.IndexOf('(');
        if (idx <= 0)
            return null;

        var close = trimmed.LastIndexOf(')');
        if (close < idx)
            return null;

        var name = trimmed[..idx].Trim();
        return string.IsNullOrWhiteSpace(name) ? null : name;
    }

    private static void WriteErrors(TextWriter writer, IEnumerable<FluentResults.IError> errors)
    {
        foreach (var e in errors)
            writer.WriteLine(e.Message);
    }
}
