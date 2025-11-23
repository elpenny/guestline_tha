namespace AssigmentApp.Types;

public record Hotel(
    string Id,
    string Name,
    List<RoomTypes> RoomTypes,
    List<Rooms> Rooms
);

public record RoomTypes(
    string Code,
    string Description,
    List<string> Amenities,
    List<string> Features
);

public record Rooms(
    string RoomType,
    string RoomId
);


