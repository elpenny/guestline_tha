namespace AssigmentApp.Types;

public record ProgramState(
    IReadOnlyList<Hotel> Hotels,
    IReadOnlyList<Booking> Bookings
);

public readonly record struct ProgramOptions(
    string HotelsPath, 
    string BookingsPath
);