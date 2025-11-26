using AssigmentApp.Types.Commands;

namespace AssigmentApp.Tests;

public class CommandParserTests
{
    [Fact]
    public void ParseAvailability_SingleDate_Succeeds()
    {
        var result = CommandParser.Parse<AvailabilityCommandArguments>("Availability(H1, 20240901, SGL)");

        Assert.True(result.IsSuccess);
        Assert.IsType<AvailabilityCommandArguments>(result.Value);
        Assert.Equal(new DateOnly(2024, 9, 1), result.Value.DateRange.Start);
        Assert.Equal(new DateOnly(2024, 9, 2), result.Value.DateRange.End);
        Assert.Equal("SGL", result.Value.RoomType);
        Assert.Equal("H1", result.Value.HotelId);
    }

    [Fact]
    public void ParseAvailability_DateRange_Succeeds()
    {
        var result = CommandParser.Parse<AvailabilityCommandArguments>("Availability(H1, 20240901-20240903, DBL)");

        Assert.True(result.IsSuccess);
        Assert.IsType<AvailabilityCommandArguments>(result.Value);
        Assert.Equal(new DateOnly(2024, 9, 1), result.Value.DateRange.Start);
        Assert.Equal(new DateOnly(2024, 9, 3), result.Value.DateRange.End);
        Assert.Equal("DBL", result.Value.RoomType);
        Assert.Equal("H1", result.Value.HotelId);
    }

    [Fact]
    public void ParseSearch_Succeeds()
    {
        var result = CommandParser.Parse<SearchCommandArguments>("Search(H1, 30, SGL)");

        Assert.True(result.IsSuccess);
        Assert.IsType<SearchCommandArguments>(result.Value);
        Assert.Equal("H1", result.Value.HotelId);
        Assert.Equal(30, result.Value.DaysToSearch);
        Assert.Equal("SGL", result.Value.RoomType);
    }

    [Fact]
    public void ParseAvailability_AllowsWhitespace()
    {
        var result = CommandParser.Parse<AvailabilityCommandArguments>("Availability( H1 , 20240901 , SGL )");

        Assert.True(result.IsSuccess);
        Assert.Equal("H1", result.Value.HotelId);
        Assert.Equal("SGL", result.Value.RoomType);
    }

    [Fact]
    public void ParseSearch_AllowsZeroDays()
    {
        var result = CommandParser.Parse<SearchCommandArguments>("Search(H1, 0, SGL)");

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.DaysToSearch);
    }

    [Fact]
    public void ParseAvailability_AllowsTrailingWhitespace()
    {
        var result = CommandParser.Parse<AvailabilityCommandArguments>("Availability(H1, 20240901, SGL)   ");

        Assert.True(result.IsSuccess);
        Assert.Equal("H1", result.Value.HotelId);
    }

    [Fact]
    public void ParseAvailability_MissingArgument_Fails()
    {
        var result = CommandParser.Parse<AvailabilityCommandArguments>("Availability(H1, 20240901)");

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void ParseAvailability_EmptyHotel_Fails()
    {
        var result = CommandParser.Parse<AvailabilityCommandArguments>("Availability( , 20240901, SGL)");

        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Unknown(H1, 20240901, SGL)")]
    [InlineData("Availability(H1, bad-date, SGL)")]
    [InlineData("Search(H1, -1, SGL)")]
    public void Parse_InvalidInputs_Fail(string input)
    {
        var result = CommandParser.Parse<AvailabilityCommandArguments>(input);
        Assert.True(result.IsFailed);
    }
}
