namespace AssigmentApp.Tests;

public class BookingDateParserTests
{
    [Fact]
    public void SingleDate_ProducesOneNightRange()
    {
        var result = BookingDateParser.ParseCommandDateOrRange("20240901");

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2024, 9, 1), result.Value.Start);
        Assert.Equal(new DateOnly(2024, 9, 2), result.Value.End);
    }

    [Fact]
    public void DateRange_ParsesArrivalInclusiveDepartureExclusive()
    {
        var result = BookingDateParser.ParseCommandDateOrRange("20240901-20240903");

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2024, 9, 1), result.Value.Start);
        Assert.Equal(new DateOnly(2024, 9, 3), result.Value.End);
    }

    [Fact]
    public void DateRange_RejectsEndBeforeStart()
    {
        var result = BookingDateParser.ParseCommandDateOrRange("20240903-20240901");

        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("2024-0901")]
    public void InvalidDate_ProducesError(string input)
    {
        var result = BookingDateParser.ParseCommandDateOrRange(input);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void BookingDates_RequireAtLeastOneNight()
    {
        var result = BookingDateParser.ParseBookingDates("20240901", "20240901");

        Assert.True(result.IsFailed);
    }
}
