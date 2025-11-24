namespace AssigmentApp.Types.Commands;

public readonly record struct AvailabilityCommandArguments(string HotelId, IReadOnlyList<DateRange> DateRanges, string RoomType);

public readonly record struct AvailabilityResult(int RoomsAvailable);
