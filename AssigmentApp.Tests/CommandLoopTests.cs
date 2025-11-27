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
        var helpCount = text.Split(ProgramConstants.HelpMessage, StringSplitOptions.None).Length - 1;
        Assert.True(helpCount >= 2);
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
        var helpCount = text.Split(ProgramConstants.HelpMessage, StringSplitOptions.None).Length - 1;
        Assert.True(helpCount >= 2);
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
        var helpCount = text.Split(ProgramConstants.HelpMessage, StringSplitOptions.None).Length - 1;
        Assert.True(helpCount >= 2);
    }
}
