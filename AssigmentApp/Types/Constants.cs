namespace AssigmentApp.Types;

public static class ProgramConstants
{
    public const string HotelsArgName = "--hotels";
    public const string BookingsArgName = "--bookings";

    public const string HelpMessage = "Program ready, possible commands are:\n" +
                                      "1. Availability(HotelName, DateRange, RoomType)\n" +
                                      "2. Search(HotelName, DaysToSearch, RoomType)\n" +
                                      "Enter blank line to exit.\n";
}
