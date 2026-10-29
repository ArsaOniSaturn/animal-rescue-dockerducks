using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DockerDucks.Api.Tests;

public class RescueReportsApiTests
{
    [Fact]
    public async Task Post_creates_prioritized_report_that_can_be_retrieved()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var response = await client.PostAsJsonAsync("/api/rescue-reports", Report("Owl", "Critical"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ReportResponse>();
        Assert.NotNull(created);
        Assert.Equal("Critical", created.Priority);
        Assert.NotEqual(Guid.Empty, created.Id);

        var retrieved = await client.GetFromJsonAsync<ReportResponse>(response.Headers.Location);
        Assert.Equal(created, retrieved);
    }

    [Fact]
    public async Task Get_returns_not_found_for_unknown_report()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var response = await client.GetAsync($"/api/rescue-reports/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("", "Downtown", "Caught in fencing", 1)]
    [InlineData("Owl", "", "Caught in fencing", 1)]
    [InlineData("Owl", "Downtown", "", 1)]
    [InlineData("Owl", "Downtown", "Caught in fencing", 0)]
    public async Task Post_rejects_invalid_input(
        string animalDescription,
        string municipalitySector,
        string situationDescription,
        int quantity)
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = Report(animalDescription, "Stable", municipalitySector, situationDescription, quantity);

        var response = await client.PostAsJsonAsync("/api/rescue-reports", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Queue_orders_by_priority_then_oldest_creation_time()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        await client.PostAsJsonAsync("/api/rescue-reports", Report("First stable animal", "Stable"));
        await client.PostAsJsonAsync("/api/rescue-reports", Report("First critical animal", "Critical"));
        await Task.Delay(5);
        await client.PostAsJsonAsync("/api/rescue-reports", Report("Second critical animal", "Stable", immediateDanger: true));
        await client.PostAsJsonAsync("/api/rescue-reports", Report("Injured animal", "Injured"));

        var queue = await client.GetFromJsonAsync<List<ReportResponse>>("/api/rescue-reports/queue");

        Assert.NotNull(queue);
        Assert.Equal(
            ["First critical animal", "Second critical animal", "Injured animal", "First stable animal"],
            queue.Select(report => report.AnimalDescription));
        Assert.Equal(["Critical", "Critical", "High", "Medium"], queue.Select(report => report.Priority));
    }

    [Fact]
    public async Task Health_and_OpenApi_are_available()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        var health = await client.GetAsync("/health");
        var openApi = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, health.StatusCode);
        Assert.Equal(HttpStatusCode.OK, openApi.StatusCode);
    }

    private static object Report(
        string animalDescription,
        string condition,
        string municipalitySector = "North sector",
        string situationDescription = "Unable to fly",
        int quantity = 1,
        bool immediateDanger = false) => new
        {
            animalDescription,
            municipalitySector,
            situationDescription,
            condition,
            immediateDanger,
            quantity
        };

    private sealed record ReportResponse(
        Guid Id,
        string AnimalDescription,
        string MunicipalitySector,
        string SituationDescription,
        string Condition,
        bool ImmediateDanger,
        int Quantity,
        string Priority,
        DateTimeOffset CreatedAt);
}
