using System.Text.Json.Serialization;
using AssigmentApp.IO;
using AssigmentApp.Parsing;

namespace AssigmentApp.Types;

public readonly record struct Booking(
    string HotelId,
    [property: JsonConverter(typeof(DateOnlyJsonConverter))]
    DateOnly Arrival,
    [property: JsonConverter(typeof(DateOnlyJsonConverter))]
    DateOnly Departure,
    string RoomType,
    string RoomRate
);
