using EChamado.Server.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace EChamado.Server.IntegrationTests.Endpoints;

[Collection("IntegrationTests")]
public class CommentsEndpointTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private const string BaseUrl = "/v1/order";
    private readonly HttpClient _client;
    private readonly HttpClient _authorizedClient;

    public CommentsEndpointTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _authorizedClient = factory.CreateClient();
        _authorizedClient.DefaultRequestHeaders.Add(TestAuthHandler.HeaderName, TestAuthHandler.HeaderValue);
    }

    [Fact]
    public async Task CreateComment_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var request = new
        {
            Text = "Comentario de teste",
            OrderId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            UserEmail = "test@example.com"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUrl}/{request.OrderId}/comments", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCommentsByOrderId_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var orderId = Guid.NewGuid();

        var response = await _client.GetAsync($"{BaseUrl}/{orderId}/comments");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateComment_WithInvalidText_ShouldReturnBadRequest(string? invalidText)
    {
        var orderId = Guid.NewGuid();
        var request = new
        {
            Text = invalidText,
            OrderId = orderId,
            UserId = Guid.NewGuid(),
            UserEmail = "test@example.com"
        };

        var response = await _authorizedClient.PostAsJsonAsync($"{BaseUrl}/{orderId}/comments", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
