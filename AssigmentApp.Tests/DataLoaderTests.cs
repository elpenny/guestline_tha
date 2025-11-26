using AssigmentApp.IO;

namespace AssigmentApp.Tests;

public class DataLoaderTests
{
    [Fact]
    public void LoadData_WithValidFiles_ReturnsState()
    {
        using var temp = new TempFolder();
        var hotelsPath = temp.WriteFile("hotels.json", ExampleHotelsJson);
        var bookingsPath = temp.WriteFile("bookings.json", ExampleBookingsJson);

        var result = DataLoader.LoadData(new ProgramOptions(hotelsPath, bookingsPath));

        Assert.True(result.IsSuccess);
        var state = result.Value;
        Assert.Single(state.Hotels);
        Assert.Equal("H1", state.Hotels[0].Id);
        Assert.Equal(4, state.Hotels[0].Rooms.Count);
        Assert.Equal(2, state.Bookings.Count);
        Assert.Equal(new DateOnly(2024, 9, 1), state.Bookings[0].Arrival);
        Assert.Equal(new DateOnly(2024, 9, 3), state.Bookings[0].Departure);
    }

    [Fact]
    public void LoadData_MissingHotelsFile_Fails()
    {
        using var temp = new TempFolder();
        var bookingsPath = temp.WriteFile("bookings.json", ExampleBookingsJson);

        var result = DataLoader.LoadData(new ProgramOptions("missing.json", bookingsPath));

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void LoadData_InvalidHotelsJson_Fails()
    {
        using var temp = new TempFolder();
        var hotelsPath = temp.WriteFile("hotels.json", "{ not valid json");
        var bookingsPath = temp.WriteFile("bookings.json", ExampleBookingsJson);

        var result = DataLoader.LoadData(new ProgramOptions(hotelsPath, bookingsPath));

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void LoadData_InvalidBookingDate_FailsGracefully()
    {
        using var temp = new TempFolder();
        var hotelsPath = temp.WriteFile("hotels.json", ExampleHotelsJson);
        var bookingsPath = temp.WriteFile("bookings.json", """
        [
          {
            "hotelId": "H1",
            "arrival": "not-a-date",
            "departure": "20240903",
            "roomType": "SGL",
            "roomRate": "Prepaid"
          }
        ]
        """);

        var result = DataLoader.LoadData(new ProgramOptions(hotelsPath, bookingsPath));

        Assert.True(result.IsFailed);
    }

    private const string ExampleHotelsJson = """
    [
      {
        "id": "H1",
        "name": "Hotel California",
        "roomTypes": [
          {
            "code": "SGL",
            "description": "Single Room",
            "amenities": [],
            "features": []
          }
        ],
        "rooms": [
          { "roomType": "SGL", "roomId": "101" },
          { "roomType": "SGL", "roomId": "102" },
          { "roomType": "SGL", "roomId": "103" },
          { "roomType": "SGL", "roomId": "104" }
        ]
      }
    ]
    """;

    private const string ExampleBookingsJson = """
    [
      {
        "hotelId": "H1",
        "arrival": "20240901",
        "departure": "20240903",
        "roomType": "SGL",
        "roomRate": "Prepaid"
      },
      {
        "hotelId": "H1",
        "arrival": "20240902",
        "departure": "20240905",
        "roomType": "SGL",
        "roomRate": "Standard"
      }
    ]
    """;

    private sealed class TempFolder : IDisposable
    {
        private readonly string _path = Directory.CreateTempSubdirectory("assigment-tests").FullName;

        public string WriteFile(string relativePath, string content)
        {
            var fullPath = Path.Combine(_path, relativePath);
            File.WriteAllText(fullPath, content);
            return fullPath;
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_path, recursive: true);
            }
            catch
            {
                // ignore cleanup issues in tests
            }
        }
    }
}
