using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DockerDucks.Api.Tests;

public class WebInterfaceTests
{
    [Theory]
    [InlineData("/")]
    [InlineData("/index.html")]
    public async Task Home_serves_the_rescue_interface_in_production(string path)
    {
        await using var application = ProductionApplication();
        using var client = application.CreateClient();

        var response = await client.GetAsync(path);
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("lang=\"es\"", html);
        Assert.Contains("Registrar rescate", html);
        Assert.Contains("Cola de atención", html);
        Assert.Contains("memoria", html);
        Assert.Contains("src=\"app.js\"", html);
        Assert.Contains("href=\"styles.css\"", html);
    }

    [Theory]
    [InlineData("/app.js", "javascript")]
    [InlineData("/styles.css", "text/css")]
    public async Task Interface_assets_are_available_in_production(string path, string contentType)
    {
        await using var application = ProductionApplication();
        using var client = application.CreateClient();

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(contentType, response.Content.Headers.ContentType?.MediaType ?? "");
        Assert.False(string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()));
    }

    [Fact]
    public async Task Unknown_api_routes_are_not_replaced_with_html()
    {
        await using var application = ProductionApplication();
        using var client = application.CreateClient();

        var response = await client.GetAsync("/api/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static WebApplicationFactory<Program> ProductionApplication() =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseEnvironment("Production"));
}
