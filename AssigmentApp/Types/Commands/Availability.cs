namespace AssigmentApp.Types.Commands;

public readonly record struct AvailabilityCommandArguments(string HotelId, DateRange DateRange, string RoomType);

public readonly record struct AvailabilityResult(int RoomsAvailable);
