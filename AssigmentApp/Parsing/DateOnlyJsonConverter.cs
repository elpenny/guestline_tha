using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssigmentApp.Parsing;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        BookingDateParser.ParseDate(reader.GetString()!);
    

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("yyyyMMdd"));
}