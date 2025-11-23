using System.Text.Json.Serialization;

namespace AssigmentApp.Types;

public record Hotel(
    string Id,
    string Name,
    IReadOnlyList<RoomType> RoomTypes,
    IReadOnlyList<Room> Rooms
);

public record RoomType(
    string Code,
    string Description,
    IReadOnlyList<string> Amenities,
    IReadOnlyList<string> Features
);

public readonly record struct Room(
    [property: JsonPropertyName("roomType")] 
    string RoomTypeCode,
    string RoomId
);



