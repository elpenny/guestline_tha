using System.Text.Json;
using AssigmentApp.Types;
using FluentResults;

namespace AssigmentApp.IO;

public static class DataLoader
{
    public static Result<ProgramState> LoadData(ProgramOptions options)
    {
        if (!File.Exists(options.HotelsPath))
            return Result.Fail($"Hotels file '{options.HotelsPath}' not found.");

        if (!File.Exists(options.BookingsPath))
            return Result.Fail($"Bookings file '{options.BookingsPath}' not found.");

        var hotelsResult = DeserializeFile<IReadOnlyList<Hotel>>(options.HotelsPath);
        if (hotelsResult.IsFailed)
            return Result.Fail(hotelsResult.Errors);

        var bookingsResult = DeserializeFile<IReadOnlyList<Booking>>(options.BookingsPath);
        if (bookingsResult.IsFailed)
            return Result.Fail(bookingsResult.Errors);

        return Result.Ok(new ProgramState(hotelsResult.Value, bookingsResult.Value));
    }

    private static Result<T> DeserializeFile<T>(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            var data = JsonSerializer.Deserialize<T>(json, SerializerOptions);
            return data is null
                ? Result.Fail($"File '{path}' did not contain valid JSON for type {typeof(T).Name}.")
                : Result.Ok(data);
        }
        catch (JsonException ex)
        {
            return Result.Fail($"Failed to parse JSON file '{path}': {ex.Message}");
        }
        catch (IOException ex)
        {
            return Result.Fail($"Failed to read file '{path}': {ex.Message}");
        }
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new DateOnlyJsonConverter() }
    };
}
