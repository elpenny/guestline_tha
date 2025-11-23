namespace AssigmentApp.Types.Commands;

public readonly record struct AvailabilityCommandArguments(IReadOnlyList<DateRange> DateRanges, string RoomType);

public readonly record struct AvailabilityResult(int RoomsAvailable);
