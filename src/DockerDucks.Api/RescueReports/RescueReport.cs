namespace DockerDucks.Api.RescueReports;

public enum RescueCondition
{
    Stable,
    Injured,
    Critical
}

public enum RescuePriority
{
    Critical,
    High,
    Medium
}

public sealed record RescueReport(
    Guid Id,
    string AnimalDescription,
    string MunicipalitySector,
    string SituationDescription,
    RescueCondition Condition,
    bool ImmediateDanger,
    int Quantity,
    RescuePriority Priority,
    DateTimeOffset CreatedAt);

public sealed record CreateRescueReportRequest(
    string? AnimalDescription,
    string? MunicipalitySector,
    string? SituationDescription,
    RescueCondition? Condition,
    bool ImmediateDanger,
    int Quantity);
