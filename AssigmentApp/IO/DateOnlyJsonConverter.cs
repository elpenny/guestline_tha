using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AssigmentApp.Parsing;

namespace AssigmentApp.IO;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var raw = reader.GetString();
        if (string.IsNullOrWhiteSpace(raw))
            throw new JsonException("Date value is empty.");

        var parsed = BookingDateParser.ParseDateResult(raw);
        if (parsed.IsFailed)
            throw new JsonException(parsed.Errors.First().Message);

        return parsed.Value;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("yyyyMMdd"));
}
