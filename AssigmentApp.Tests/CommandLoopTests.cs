using System.Text;

namespace AssigmentApp.Tests;

public class CommandLoopTests
{
    [Fact]
    public void AvailabilityCommand_PrintsAvailability()
    {
        var state = TestDataFactory.CreateValidState();
        var input = new StringReader("Availability(H1, 20240902, SGL)\n\n");
        var output = new StringWriter(new StringBuilder());

        CommandLoop.Run(input, output, state);

        var text = output.ToString();
        Assert.Contains(ProgramConstants.HelpMessage.Trim(), text);
        Assert.Contains("\n1\n", text);
    }

    [Fact]
    public void UnknownCommand_PrintsError()
    {
        var state = TestDataFactory.CreateValidState();
        var input = new StringReader("Unknown(H1, 20240902, SGL)\n\n");
        var output = new StringWriter(new StringBuilder());

        CommandLoop.Run(input, output, state);

        var text = output.ToString();
        Assert.Contains("Unknown command 'Unknown'.", text);
    }

    [Fact]
    public void InvalidFormat_PrintsError()
    {
        var state = TestDataFactory.CreateValidState();
        var input = new StringReader("bad command\n\n");
        var output = new StringWriter(new StringBuilder());

        CommandLoop.Run(input, output, state);

        var text = output.ToString();
        Assert.Contains("Invalid command format", text);
    }
}
