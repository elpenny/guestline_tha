namespace AssigmentApp.Tests;

public class DataValidatorTests
{
    [Fact]
    public void ValidState_PassesValidation()
    {
        var state = TestDataFactory.CreateValidState();

        var result = DataValidator.Validate(state);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void BookingForUnknownHotel_FailsValidation()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            TestDataFactory.CreateBooking("MissingHotel", "SGL", "20240901", "20240902")
        });

        var result = DataValidator.Validate(state);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void BookingWithUnknownRoomType_FailsValidation()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            TestDataFactory.CreateBooking("H1", "MISSING", "20240901", "20240902")
        });

        var result = DataValidator.Validate(state);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void RoomWithUnknownRoomType_FailsValidation()
    {
        var invalidRooms = new List<Room>
        {
            new Room("UNKNOWN", "101")
        };

        var state = TestDataFactory.CreateValidState(rooms: invalidRooms);

        var result = DataValidator.Validate(state);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void BookingWithZeroNights_FailsValidation()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            TestDataFactory.CreateBooking("H1", "SGL", "20240901", "20240901")
        });

        var result = DataValidator.Validate(state);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void DuplicateHotelIds_FailsValidation()
    {
        var roomType = new RoomType("SGL", "Single", Array.Empty<string>(), Array.Empty<string>());
        var rooms = new List<Room> { new Room(roomType.Code, "101") };

        var hotel1 = new Hotel("H1", "Hotel One", new[] { roomType }, rooms);
        var hotel2 = new Hotel("h1", "Hotel Two", new[] { roomType }, rooms);

        var state = new ProgramState(new List<Hotel> { hotel1, hotel2 }, new List<Booking>());

        var result = DataValidator.Validate(state);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void BookingHotelId_IsCaseInsensitive()
    {
        var state = TestDataFactory.CreateValidState(bookings: new[]
        {
            TestDataFactory.CreateBooking("h1", "SGL", "20240901", "20240902")
        });

        var result = DataValidator.Validate(state);

        Assert.True(result.IsSuccess);
    }
}
