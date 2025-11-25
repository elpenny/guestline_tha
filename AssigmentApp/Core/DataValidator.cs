using AssigmentApp.Types;
using FluentResults;

namespace AssigmentApp.Core;

public static class DataValidator
{
    public static Result Validate(ProgramState state)
    {
        var errors = new List<string>();
        var hotelsById = state.Hotels.ToDictionary(h => h.Id);

        foreach (var booking in state.Bookings)
        {
            if (!hotelsById.TryGetValue(booking.HotelId, out var hotel))
            {
                errors.Add($"Booking references unknown hotel '{booking.HotelId}'.");
                continue;
            }

            var roomTypeCodes = hotel.RoomTypes.Select(rt => rt.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!roomTypeCodes.Contains(booking.RoomType))
                errors.Add($"Booking references unknown room type '{booking.RoomType}' for hotel '{booking.HotelId}'.");

            if (booking.Departure <= booking.Arrival)
                errors.Add($"Booking for hotel '{booking.HotelId}' has invalid dates: departure {booking.Departure:yyyyMMdd} is not after arrival {booking.Arrival:yyyyMMdd}.");
        }

        foreach (var hotel in state.Hotels)
        {
            var roomTypeCodes = hotel.RoomTypes.Select(rt => rt.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var room in hotel.Rooms)
            {
                if (!roomTypeCodes.Contains(room.RoomTypeCode))
                    errors.Add($"Room '{room.RoomId}' in hotel '{hotel.Id}' references unknown room type '{room.RoomTypeCode}'.");
            }
        }

        return errors.Count > 0
            ? Result.Fail(errors)
            : Result.Ok();
    }
}
