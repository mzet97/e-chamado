using System.Net;
using System.Net.Http.Json;
using EChamado.Server.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.IntegrationTests.Endpoints;

public class HealthCheckTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ShouldReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
        content.Should().Contain("postgresql");
        content.Should().Contain("redis");
    }

    [Fact]
    public async Task HealthReady_ShouldReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task HealthLive_ShouldReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
