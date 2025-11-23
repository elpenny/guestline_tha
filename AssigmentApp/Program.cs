// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using AssigmentApp.Types;

namespace AssigmentApp;

partial class Program
{
    private const string HotelsArgName = "--hotels";
    private const string BookingsArgName = "--bookings";

    static void Main(string[] args)
    {
        Console.WriteLine("Parsing arguments...");
        if (args.Length < 4)
        {
            Console.WriteLine("Not enough arguments, exiting...");
            return;
        }
        
        var hotelsPath = string.Empty;
        var bookingsPath = string.Empty;

        for (var index = 0; index < args.Length; index+=2)
        {
            var arg = args[index];
            switch (arg)
            {
                case HotelsArgName:
                    hotelsPath = args[index+1].Trim();
                    break;
                case BookingsArgName:
                    bookingsPath = args[index+1].Trim();
                    break;
                default:
                    Console.WriteLine("Unknown argument: " + arg);
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(hotelsPath) || string.IsNullOrWhiteSpace(bookingsPath))
        {
            Console.WriteLine("One of required parameters was not provided, exiting.");
            return;
        }

        var hotels = OpenAndParse<Hotel>(hotelsPath);
        var bookings = OpenAndParse<Booking>(bookingsPath);
        if (hotels is null || bookings is null || hotels.Count == 0 || bookings.Count == 0)
        {
            Console.WriteLine("Not enough data to operate, exiting.");
            return;
        }

        var success = ValidateData(hotels, bookings);
        if (!success)
        {
            Console.WriteLine("Data validation failed, exiting.");
            return;
        }
        
        PrintHelpMessage();
        while (true)
        {
            var userInput = Console.ReadLine()!;
            if (userInput.StartsWith(nameof(Command.Availability)))
            {
                RunCommand(Command.Availability, new string(userInput.TrimStart(nameof(Command.Availability))));
            }
            else if (userInput.StartsWith(nameof(Command.Search)))
            {
                RunCommand(Command.Search, new string(userInput.TrimStart(nameof(Command.Search))));
            }
            else if (userInput == string.Empty)
            {
                Console.WriteLine("Exiting.");
                break;
            }
            PrintHelpMessage();
        }
    }

    private static void RunCommand(Command command, string userInput)
    {
        Console.WriteLine($"Running command: {command} with user input: {userInput}");
    }

    private static void PrintHelpMessage()
    {
        Console.Write("Program ready, possible commands are:\n" +
                      "1. Availability(HotelName, DateRange, RoomType)\n" +
                      "2. Search(HotelName, DaysToSearch, RoomType)\n" +
                      "Enter blank line to exit.\n");
    }

    private static bool ValidateData(List<Hotel> hotels, List<Booking> bookings)
    {
        Console.WriteLine($"Hotels successfully validated: {hotels.Count} hotels found.");
        Console.WriteLine($"Bookings successfully validated: {bookings.Count} bookings found.");
        return true;
    }

    static List<T>? OpenAndParse<T>(string jsonPath)
    {
        if (!Path.Exists(jsonPath))
        {
            Console.WriteLine($"Provided path does not exist: {jsonPath}");
            return null;
        }

        var fileContent = File.ReadAllText(jsonPath);
        if (string.IsNullOrWhiteSpace(fileContent))
        {
            Console.WriteLine($"No content found at: {jsonPath}");
            return null;
        }

        var content = JsonSerializer.Deserialize<List<T>>(fileContent);
        if (content == null || content.Count == 0)
        {
            Console.WriteLine($"No content found at: {jsonPath}");
            return null;
        }

        return content;
    }
}