using System.Text.Json.Serialization;
using DockerDucks.Api.RescueReports;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RescueReportRepository>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/api/rescue-reports", (
    CreateRescueReportRequest request,
    RescueReportRepository repository) =>
{
    var errors = Validate(request);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var condition = request.Condition!.Value;
    var report = new RescueReport(
        Guid.NewGuid(),
        request.AnimalDescription!.Trim(),
        request.MunicipalitySector!.Trim(),
        request.SituationDescription!.Trim(),
        condition,
        request.ImmediateDanger,
        request.Quantity,
        PriorityCalculator.Determine(condition, request.ImmediateDanger),
        DateTimeOffset.UtcNow);

    repository.Add(report);
    return Results.Created($"/api/rescue-reports/{report.Id}", report);
})
.WithName("CreateRescueReport")
.WithOpenApi();

app.MapGet("/api/rescue-reports/queue", (RescueReportRepository repository) =>
        Results.Ok(repository.GetQueue()))
    .WithName("GetRescueReportQueue")
    .WithOpenApi();

app.MapGet("/api/rescue-reports/{id:guid}", (Guid id, RescueReportRepository repository) =>
        repository.Get(id) is { } report ? Results.Ok(report) : Results.NotFound())
    .WithName("GetRescueReport")
    .WithOpenApi();

app.MapHealthChecks("/health");

app.Run();

static Dictionary<string, string[]> Validate(CreateRescueReportRequest request)
{
    var errors = new Dictionary<string, string[]>();

    AddRequiredError(request.AnimalDescription, nameof(request.AnimalDescription));
    AddRequiredError(request.MunicipalitySector, nameof(request.MunicipalitySector));
    AddRequiredError(request.SituationDescription, nameof(request.SituationDescription));

    if (request.Condition is null)
    {
        errors[nameof(request.Condition)] = ["Condition is required."];
    }

    if (request.Quantity <= 0)
    {
        errors[nameof(request.Quantity)] = ["Quantity must be greater than zero."];
    }

    return errors;

    void AddRequiredError(string? value, string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors[field] = [$"{field} is required."];
        }
    }
}

public partial class Program;
