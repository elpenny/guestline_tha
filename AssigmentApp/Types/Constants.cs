namespace AssigmentApp.Types;

public static class ProgramConstants
{
    private const string HotelsArgName = "--hotels";
    private const string BookingsArgName = "--bookings";

    private const string HelpMessage = "Program ready, possible commands are:\n" +
                                       "1. Availability(HotelName, DateRange, RoomType)\n" +
                                       "2. Search(HotelName, DaysToSearch, RoomType)\n" +
                                       "Enter blank line to exit.\n";
}

public enum Command
{
    Availability,
    Search,
}