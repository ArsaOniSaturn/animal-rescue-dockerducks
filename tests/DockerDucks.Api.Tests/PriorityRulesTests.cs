using DockerDucks.Api.RescueReports;

namespace DockerDucks.Api.Tests;

public class PriorityRulesTests
{
    [Theory]
    [InlineData(RescueCondition.Critical, false, RescuePriority.Critical)]
    [InlineData(RescueCondition.Stable, true, RescuePriority.Critical)]
    [InlineData(RescueCondition.Injured, false, RescuePriority.High)]
    [InlineData(RescueCondition.Stable, false, RescuePriority.Medium)]
    public void Determine_returns_expected_priority(
        RescueCondition condition,
        bool immediateDanger,
        RescuePriority expected)
    {
        var priority = PriorityCalculator.Determine(condition, immediateDanger);

        Assert.Equal(expected, priority);
    }
}
