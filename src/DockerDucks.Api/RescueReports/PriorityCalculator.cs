namespace DockerDucks.Api.RescueReports;

public static class PriorityCalculator
{
    public static RescuePriority Determine(RescueCondition condition, bool immediateDanger) =>
        condition switch
        {
            RescueCondition.Critical => RescuePriority.Critical,
            _ when immediateDanger => RescuePriority.Critical,
            RescueCondition.Injured => RescuePriority.High,
            _ => RescuePriority.Medium
        };
}
