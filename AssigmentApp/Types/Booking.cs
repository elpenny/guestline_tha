using System.Text.Json.Serialization;
using AssigmentApp.CustomConverters;

namespace AssigmentApp.Types;

public record Booking(
    string HotelId,
    [property: JsonConverter(typeof(DateOnlyJsonConverter))]
    DateOnly Arrival,
    [property: JsonConverter(typeof(DateOnlyJsonConverter))]
    DateOnly Departure,
    string RoomType,
    string RoomRate
);

