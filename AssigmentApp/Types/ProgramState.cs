namespace AssigmentApp.Types;

public record ProgramState(
    List<Hotel> Hotels,
    List<Booking> Bookings
);

public record ProgramOptions(
    string HotelsPath, 
    string BookingsPath
);