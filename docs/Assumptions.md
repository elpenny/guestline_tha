### Assumptions

1. If loading input files fails (missing files, invalid JSON, failed validation), the program exits with an error and does not attempt recovery.
2. Only basic structural validations are performed:
    * No bookings for hotels that are not present in `hotels.json`.
    * No rooms whose `roomType` does not exist in the hotel's `roomTypes` list.
    * No bookings referencing a `roomType` not present in the hotel's `roomTypes` list.
      Any validation failure results in program termination.
3. All dates use the `yyyyMMdd` format.
   Bookings treat `arrival` as inclusive and `departure` as exclusive (a booking from `20240901` to `20240903` blocks nights on the 1st and 2nd; the room becomes free on the 3rd).
4. A single-date availability query such as `Availability(H1, 20240901, SGL)` is interpreted as checking the night of 2024-09-01 (arrival = 20240901, departure = 20240902).
5. A date range in commands, e.g. `20240901-20240903`, is interpreted as arrival on 2024-09-01, departure on 2024-09-03 (arrival inclusive, departure exclusive).
6. Each record in `bookings.json` represents exactly one booked room; there is no quantity field.
7. `roomRate` is read but does not influence availability calculations.
8. For each hotel, capacity for a room type is defined as the number of entries in the `rooms` list with that `roomType`.
   Overbookings are allowed, so availability may be negative if bookings exceed capacity.
9. In `Search(H1, N, SGL)`, the value `N` represents the number of days ahead starting from the current system date (`DateTime.Today`).
10. JSON input files must match the expected structure. Unknown fields are ignored, but missing or malformed required fields cause an error.
11. The program loads all hotel and booking data into memory at startup; the dataset is assumed small enough for this approach.
12. Only the commands `Availability(...)` and `Search(...)` are supported.
    Whitespaces around arguments are ignored, but the overall command syntax must match the expected format.
13. Invalid console commands (wrong syntax, unrecognized hotel IDs or room types, invalid dates, negative day ranges, etc.) print an error message but do not terminate the program.
    The application stops only when an empty line is entered.
14. Availability for a single date is calculated as:
    `capacity(roomType) - number_of_bookings_for_that_date`.
15. Availability for a date range is the minimum availability across all dates in that range (the maximum number of rooms that can be booked for the entire period).
16. The `Search` command scans days from today (inclusive) up to `today + N` (exclusive) and produces maximal contiguous ranges where availability is strictly positive for the entire range.
17. Each search result range reports the minimum availability across that range (the guaranteed availability for the full period).
18. Time-of-day and time-zone considerations are ignored; the logic is purely date-based.
19. The application is a single-user, offline command-line tool. JSON files are assumed not to change while the program is running.
