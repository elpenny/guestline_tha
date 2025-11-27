using AssigmentApp.Core;
using AssigmentApp.Types;
using AssigmentApp.Types.Commands;

namespace AssigmentApp.Tests;

public class CommandHandlerTests
{
    [Fact]
    public void Availability_ReturnsCapacityMinusBookings()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            TestDataFactory.CreateBooking("H1", "SGL", "20240901", "20240903")
        });

        var args = new AvailabilityCommandArguments(
            "H1",
            new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 2)),
            "SGL");

        var result = CommandHandler.HandleAvailability(state, args);

        Assert.True(result.IsSuccess);
        // Capacity 2 (rooms 101,102) minus 1 booking => 1
        Assert.Equal(1, result.Value.RoomsAvailable);
    }

    [Fact]
    public void Availability_WithEmptyRange_Fails()
    {
        var state = TestDataFactory.CreateValidState();
        var args = new AvailabilityCommandArguments(
            "H1",
            new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 1)),
            "SGL");

        var result = CommandHandler.HandleAvailability(state, args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Availability_ForUnknownHotel_Fails()
    {
        var state = TestDataFactory.CreateValidState();
        var args = new AvailabilityCommandArguments(
            "Missing",
            new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 2)),
            "SGL");

        var result = CommandHandler.HandleAvailability(state, args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Availability_ForUnknownRoomType_Fails()
    {
        var state = TestDataFactory.CreateValidState();
        var args = new AvailabilityCommandArguments(
            "H1",
            new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 2)),
            "MISSING");

        var result = CommandHandler.HandleAvailability(state, args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Availability_MinimumAcrossRangeIsReturned()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            // Occupy both rooms on 2nd; only one room on 1st
            TestDataFactory.CreateBooking("H1", "SGL", "20240901", "20240902"),
            TestDataFactory.CreateBooking("H1", "SGL", "20240902", "20240903")
        });

        var args = new AvailabilityCommandArguments(
            "H1",
            new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 3)),
            "SGL");

        var result = CommandHandler.HandleAvailability(state, args);

        Assert.True(result.IsSuccess);
        // Day 1 availability: 1; Day 2 availability: 1 -> min = 1
        Assert.Equal(1, result.Value.RoomsAvailable);
    }

    [Fact]
    public void Availability_CanReturnNegativeWhenOverbooked()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            // Two rooms exist; book three overlapping stays
            TestDataFactory.CreateBooking("H1", "SGL", "20240901", "20240903"),
            TestDataFactory.CreateBooking("H1", "SGL", "20240901", "20240903"),
            TestDataFactory.CreateBooking("H1", "SGL", "20240902", "20240903")
        });

        var args = new AvailabilityCommandArguments(
            "H1",
            new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 3)),
            "SGL");

        var result = CommandHandler.HandleAvailability(state, args);

        Assert.True(result.IsSuccess);
        // Capacity 2 minus 3 bookings => -1 indicates overbooking
        Assert.Equal(-1, result.Value.RoomsAvailable);
    }

    [Fact]
    public void Search_FindsContiguousAvailableRanges()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            // Occupied on 1st and 4th; free otherwise within first 5 days
            TestDataFactory.CreateBooking("H1", "SGL", "20240901", "20240902"),
            TestDataFactory.CreateBooking("H1", "SGL", "20240904", "20240905")
        });

        var args = new SearchCommandArguments("H1", 5, "SGL");

        var result = CommandHandler.HandleSearch(
            state,
            args,
            today: new DateOnly(2024, 9, 1));

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Entries);
        Assert.Equal(new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 6)), result.Value.Entries[0].Range);
        Assert.Equal(1, result.Value.Entries[0].RoomsAvailable);
    }

    [Fact]
    public void Search_ForUnknownHotel_Fails()
    {
        var state = TestDataFactory.CreateValidState();
        var args = new SearchCommandArguments("Missing", 5, "SGL");

        var result = CommandHandler.HandleSearch(state, args, today: new DateOnly(2024, 9, 1));

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Search_ForUnknownRoomType_Fails()
    {
        var state = TestDataFactory.CreateValidState();
        var args = new SearchCommandArguments("H1", 5, "MISSING");

        var result = CommandHandler.HandleSearch(state, args, today: new DateOnly(2024, 9, 1));

        Assert.True(result.IsFailed);
    }
}
