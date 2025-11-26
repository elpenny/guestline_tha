using System.Text.Json;
using AssigmentApp.IO;

namespace AssigmentApp.Tests;

public class DateOnlyJsonConverterTests
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new DateOnlyJsonConverter() }
    };

    private sealed record Container(DateOnly Date);

    [Fact]
    public void Serialize_WritesExpectedFormat()
    {
        var json = JsonSerializer.Serialize(new Container(new DateOnly(2024, 9, 1)), _options);

        Assert.Equal("{\"Date\":\"20240901\"}", json);
    }

    [Fact]
    public void Deserialize_ReadsExpectedDate()
    {
        var result = JsonSerializer.Deserialize<Container>("{\"Date\":\"20240901\"}", _options);

        Assert.NotNull(result);
        Assert.Equal(new DateOnly(2024, 9, 1), result!.Date);
    }

    [Fact]
    public void Deserialize_InvalidFormat_Throws()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Container>("{\"Date\":\"09-01-2024\"}", _options));
    }
}
