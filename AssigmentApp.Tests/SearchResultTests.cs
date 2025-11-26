using AssigmentApp.Types.Commands;

namespace AssigmentApp.Tests;

public class SearchResultTests
{
    [Fact]
    public void ToString_IncludesEndDateForSingleNightRanges()
    {
        var entry = new SearchResultEntry(
            new DateRange(new DateOnly(2024, 9, 1), new DateOnly(2024, 9, 2)),
            2);

        var text = new SearchResult(new[] { entry }).ToString();

        Assert.Equal("(20240901-20240902, 2)", text);
    }
}
