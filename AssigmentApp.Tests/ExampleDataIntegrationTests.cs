using AssigmentApp.Core;
using AssigmentApp.IO;
using AssigmentApp.Types;
using AssigmentApp.Types.Commands;

namespace AssigmentApp.Tests;

public class ExampleDataIntegrationTests
{
    private static readonly Lazy<ProgramState> ExampleState =
        new(() => LoadState("hotels.json", "bookings.json"));

    private static readonly Lazy<ProgramState> ComplexExampleState =
        new(() => LoadState("hotels_complex.json", "bookings_complex.json"));

    [Fact]
    public void Availability_WithExampleData_ReturnsAvailableRooms()
    {
        var state = LoadExampleState();
        var range = new DateRange(new DateOnly(2024, 9, 3), new DateOnly(2024, 9, 5));
        var args = new AvailabilityCommandArguments("H1", range, "DBL");

        var result = CommandsHandler.HandleAvailability(state, args);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.RoomsAvailable);
    }

    [Fact]
    public void Availability_WithUnknownRoomType_Fails()
    {
        var state = LoadExampleState();
        var range = new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 2));
        var args = new AvailabilityCommandArguments("H1", range, "XXX");

        var result = CommandsHandler.HandleAvailability(state, args);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Search_WithExampleData_ReturnsSingleAvailabilityRun()
    {
        var state = LoadExampleState();
        var args = new SearchCommandArguments("H1", 5, "SGL");

        var result = CommandsHandler.HandleSearch(state, args, new DateOnly(2024, 9, 1));

        Assert.True(result.IsSuccess);
        var entry = Assert.Single(result.Value.Entries);
        Assert.Equal(new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 6)), entry.Range);
        Assert.Equal(1, entry.RoomsAvailable);
    }

    [Fact]
    public void Search_WithUnknownHotel_Fails()
    {
        var state = LoadExampleState();
        var args = new SearchCommandArguments("NOPE", 3, "SGL");

        var result = CommandsHandler.HandleSearch(state, args, new DateOnly(2024, 9, 1));

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Availability_WithComplexExampleData_AllDeluxesBookedYieldsZero()
    {
        var state = LoadComplexExampleState();
        var range = new DateRange(new DateOnly(2024, 10, 12), new DateOnly(2024, 10, 13));
        var args = new AvailabilityCommandArguments("H1", range, "DLX");

        var result = CommandsHandler.HandleAvailability(state, args);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.RoomsAvailable);
    }

    [Fact]
    public void Search_WithComplexExampleData_ReturnsMinAcrossBusyWeek()
    {
        var state = LoadComplexExampleState();
        var args = new SearchCommandArguments("H2", 7, "QLN");

        var result = CommandsHandler.HandleSearch(state, args, new DateOnly(2024, 10, 5));

        Assert.True(result.IsSuccess);
        var entry = Assert.Single(result.Value.Entries);
        Assert.Equal(new DateRange(new DateOnly(2024, 10, 5), new DateOnly(2024, 10, 12)), entry.Range);
        Assert.Equal(2, entry.RoomsAvailable);
    }

    private static ProgramState LoadExampleState() => ExampleState.Value;
    private static ProgramState LoadComplexExampleState() => ComplexExampleState.Value;

    private static ProgramState LoadState(string hotelsFile, string bookingsFile)
    {
        var hotelsPath = GetExamplePath(hotelsFile);
        var bookingsPath = GetExamplePath(bookingsFile);

        if (!File.Exists(hotelsPath))
            throw new FileNotFoundException($"Expected example hotels file at '{hotelsPath}'");
        if (!File.Exists(bookingsPath))
            throw new FileNotFoundException($"Expected example bookings file at '{bookingsPath}'");

        var result = DataLoader.LoadData(new ProgramOptions(hotelsPath, bookingsPath));
        if (result.IsFailed)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Message));
            throw new InvalidOperationException($"Failed to load example data: {errors}");
        }

        return result.Value;
    }

    private static string GetExamplePath(string fileName) =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "example", fileName));
}
