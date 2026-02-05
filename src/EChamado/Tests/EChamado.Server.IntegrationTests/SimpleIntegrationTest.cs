using System.Net;
using EChamado.Server.IntegrationTests.Infrastructure;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.IntegrationTests;

public class SimpleIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public SimpleIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Simple_Endpoint_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}