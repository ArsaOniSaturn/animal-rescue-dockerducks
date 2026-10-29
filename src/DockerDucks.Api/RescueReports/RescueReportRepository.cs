using System.Collections.Concurrent;

namespace DockerDucks.Api.RescueReports;

public sealed class RescueReportRepository
{
    private readonly ConcurrentDictionary<Guid, RescueReport> _reports = new();

    public RescueReport Add(RescueReport report)
    {
        if (!_reports.TryAdd(report.Id, report))
        {
            throw new InvalidOperationException($"A report with id {report.Id} already exists.");
        }

        return report;
    }

    public RescueReport? Get(Guid id) =>
        _reports.GetValueOrDefault(id);

    public IReadOnlyList<RescueReport> GetQueue() =>
        _reports.Values
            .OrderBy(report => PriorityRank(report.Priority))
            .ThenBy(report => report.CreatedAt)
            .ToArray();

    private static int PriorityRank(RescuePriority priority) => priority switch
    {
        RescuePriority.Critical => 0,
        RescuePriority.High => 1,
        RescuePriority.Medium => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null)
    };
}
