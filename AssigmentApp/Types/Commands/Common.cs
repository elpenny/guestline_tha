namespace AssigmentApp.Types.Commands;

public enum CommandType
{
    Availability,
    Search,
}

public readonly record struct DateRange(DateOnly Start, DateOnly End)
{
    public bool IsSingleDay => Start == End;
}
