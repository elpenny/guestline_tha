namespace AssigmentApp.Types;

public record Booking(
    string HotelId,
    DateOnly Arrival,
    DateOnly Departure,
    string RoomType,
    string RoomRate
);

