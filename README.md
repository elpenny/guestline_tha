# Synopsis

Take Home Assigment for Guestline - short program for hotel rooms bookings management with minimal feature set.

## How to build and run

Prerequisites:
1. .NET 10 SDK installed
2. Input JSON files (examples provided under `example/`)

Steps:
1. Check out the repo
2. From repo root, run:  
   `dotnet run --project AssigmentApp --hotels example/hotels.json --bookings example/bookings.json`
3. The app will print the command help; use `Availability(...)` and `Search(...)` commands. Press Enter on an empty line to exit.

Example datasets:
- `example/hotels.json` and `example/bookings.json`: single-hotel, minimal starter set.
- `example/hotels_complex.json` and `example/bookings_complex.json`: multi-hotel set with overlapping bookings to exercise availability/search edge cases.

## How to test
1. Check out repo
2. From repo root, run:  
   `dotnet test`
3. All tests should pass.

## Notes on AI assistance
The initial exploration, control flow design, component boundaries, and public signatures were written by me. AI assistance was used to fill in the implementation and unit tests that accompany this project.
