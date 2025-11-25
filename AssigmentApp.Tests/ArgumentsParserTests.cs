namespace AssigmentApp.Tests;

public class ArgumentsParserTests
{
    [Fact]
    public void Parse_WithBothPaths_Succeeds()
    {
        var args = new[]
        {
            ProgramConstants.HotelsArgName, "hotels.json",
            ProgramConstants.BookingsArgName, "bookings.json"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsSuccess);
        Assert.Equal("hotels.json", result.Value.HotelsPath);
        Assert.Equal("bookings.json", result.Value.BookingsPath);
    }

    [Fact]
    public void Parse_MissingHotels_Fails()
    {
        var args = new[]
        {
            ProgramConstants.BookingsArgName, "bookings.json"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Parse_MissingBookings_Fails()
    {
        var args = new[]
        {
            ProgramConstants.HotelsArgName, "hotels.json"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Parse_UnknownArgument_Fails()
    {
        var args = new[]
        {
            "--unknown", "value",
            ProgramConstants.HotelsArgName, "hotels.json",
            ProgramConstants.BookingsArgName, "bookings.json"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Parse_ArgumentWithoutValue_Fails()
    {
        var args = new[]
        {
            ProgramConstants.HotelsArgName,
            ProgramConstants.BookingsArgName, "bookings.json"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Parse_TooManyArguments_Fails()
    {
        var args = new[]
        {
            ProgramConstants.HotelsArgName, "hotels.json",
            ProgramConstants.BookingsArgName, "bookings.json",
            "extra"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Parse_NoArguments_Fails()
    {
        var args = Array.Empty<string>();

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Parse_DuplicateHotels_Fails()
    {
        var args = new[]
        {
            ProgramConstants.HotelsArgName, "h1.json",
            ProgramConstants.HotelsArgName, "h2.json",
            ProgramConstants.BookingsArgName, "bookings.json"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Parse_DuplicateBookings_Fails()
    {
        var args = new[]
        {
            ProgramConstants.HotelsArgName, "hotels.json",
            ProgramConstants.BookingsArgName, "b1.json",
            ProgramConstants.BookingsArgName, "b2.json"
        };

        var result = ArgumentsParser.Parse(args);

        Assert.True(result.IsFailed);
    }
}
