using AssigmentApp.Types;
using AssigmentApp.Types.Commands;
using FluentResults;

namespace AssigmentApp.Core;

public static class CommandsHandler
{
    public static Result<AvailabilityResult> HandleAvailability(
        ProgramState state,
        AvailabilityCommandArguments args)
    {
        if (args.DateRange.End <= args.DateRange.Start)
            return Result.Fail("Availability date range must end after it starts.");

        var hotel = FindHotel(state, args.HotelId);
        if (hotel is null)
            return Result.Fail($"Unknown hotel '{args.HotelId}'.");

        if (!HotelHasRoomType(hotel, args.RoomType))
            return Result.Fail($"Unknown room type '{args.RoomType}' for hotel '{args.HotelId}'.");

        var capacity = RoomTypeCapacity(hotel, args.RoomType);
        var availability = CalculateMinAvailability(state.Bookings, args.HotelId, args.RoomType, args.DateRange, capacity);

        return Result.Ok(new AvailabilityResult(availability));
    }

    public static Result<SearchResult> HandleSearch(
        ProgramState state,
        SearchCommandArguments args,
        DateOnly? today = null)
    {
        var hotel = FindHotel(state, args.HotelId);
        if (hotel is null)
            return Result.Fail($"Unknown hotel '{args.HotelId}'.");

        if (!HotelHasRoomType(hotel, args.RoomType))
            return Result.Fail($"Unknown room type '{args.RoomType}' for hotel '{args.HotelId}'.");

        var capacity = RoomTypeCapacity(hotel, args.RoomType);
        var baseDate = today ?? DateOnly.FromDateTime(DateTime.Today);
        if (args.DaysToSearch <= 0)
            return Result.Ok(new SearchResult(Array.Empty<SearchResultEntry>()));

        var startDate = baseDate;
        var endDate = baseDate.AddDays(args.DaysToSearch);

        var entries = new List<SearchResultEntry>();
        DateOnly? currentStart = null;
        var currentMin = int.MaxValue;

        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            var dayRange = new DateRange(date, date.AddDays(1));
            var availability = CalculateMinAvailability(state.Bookings, args.HotelId, args.RoomType, dayRange, capacity);

            if (availability > 0)
            {
                if (currentStart is null)
                {
                    currentStart = date;
                    currentMin = availability;
                }
                else
                {
                    currentMin = Math.Min(currentMin, availability);
                }
            }
            else if (currentStart is not null)
            {
                entries.Add(new SearchResultEntry(new DateRange(currentStart.Value, date), currentMin));
                currentStart = null;
                currentMin = int.MaxValue;
            }
        }

        if (currentStart is not null)
            entries.Add(new SearchResultEntry(new DateRange(currentStart.Value, endDate), currentMin));

        return Result.Ok(new SearchResult(entries));
    }

    private static Hotel? FindHotel(ProgramState state, string hotelId) =>
        state.Hotels.FirstOrDefault(h => string.Equals(h.Id, hotelId, StringComparison.OrdinalIgnoreCase));

    private static bool HotelHasRoomType(Hotel hotel, string roomType) =>
        hotel.RoomTypes.Any(rt => string.Equals(rt.Code, roomType, StringComparison.OrdinalIgnoreCase));

    private static int RoomTypeCapacity(Hotel hotel, string roomType) =>
        hotel.Rooms.Count(r => string.Equals(r.RoomTypeCode, roomType, StringComparison.OrdinalIgnoreCase));

    private static int CalculateMinAvailability(
        IReadOnlyList<Booking> bookings,
        string hotelId,
        string roomType,
        DateRange range,
        int capacity)
    {
        var min = int.MaxValue;
        for (var date = range.Start; date < range.End; date = date.AddDays(1))
        {
            var busy = bookings.Count(b =>
                string.Equals(b.HotelId, hotelId, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(b.RoomType, roomType, StringComparison.OrdinalIgnoreCase) &&
                b.Arrival <= date &&
                b.Departure > date);

            var available = capacity - busy;
            min = Math.Min(min, available);
        }

        return min == int.MaxValue ? capacity : min;
    }
}
