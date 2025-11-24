namespace AssigmentApp.Tests;

internal static class TestDataFactory
{
    private const string DefaultHotelId = "H1";

    public static ProgramState CreateValidState(
        string hotelId = DefaultHotelId,
        IEnumerable<RoomType>? roomTypes = null,
        IEnumerable<Room>? rooms = null,
        IEnumerable<Booking>? bookings = null)
    {
        var defaultRoomType = new RoomType(
            "SGL",
            "Single",
            Array.Empty<string>(),
            Array.Empty<string>());

        var rtList = roomTypes?.ToList() ?? new List<RoomType> { defaultRoomType };
        var roomList = rooms?.ToList() ?? new List<Room>
        {
            new Room(defaultRoomType.Code, "101"),
            new Room(defaultRoomType.Code, "102"),
        };

        var defaultBookings = bookings?.ToList() ?? new List<Booking>
        {
            CreateBooking(hotelId, rtList[0].Code, "20240901", "20240903")
        };

        var hotel = new Hotel(hotelId, "Sample Hotel", rtList, roomList);

        return new ProgramState(new List<Hotel> { hotel }, defaultBookings);
    }

    public static Booking CreateBooking(
        string hotelId,
        string roomType,
        string arrival,
        string departure)
    {
        return new Booking(
            hotelId,
            BookingDateParser.ParseDate(arrival),
            BookingDateParser.ParseDate(departure),
            roomType,
            "Standard");
    }
}
