namespace AssigmentApp.Types;

public record SearchCommandArguments(string HotelName, int DaysToSearch, string RoomType);

public record AvailabilityCommandArguments(string HotelName, DateRange DateRange, string RoomType);

public record DateRange(DateOnly Start, DateOnly End)
{
    public bool IsSingleDay => Start == End;
}