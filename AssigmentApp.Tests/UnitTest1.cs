using Xunit.Abstractions;

namespace AssigmentApp.Tests;

public class UnitTest1(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void Test1()
    {
        testOutputHelper.WriteLine("Hello, World!");
    }
}