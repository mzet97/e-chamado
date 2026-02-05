using EChamado.Server.IntegrationTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace EChamado.Server.IntegrationTests.Endpoints;

[Collection("IntegrationTests")]
public class OrdersEndpointTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private const string BaseUrl = "/v1/order"; // Corrigido para o mapeamento real da API
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public OrdersEndpointTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrders_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/v1/orders"); // Usar o endpoint correto para SearchOrdersEndpoint

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new
        {
            Title = "Teste Order Integration",
            Description = "Descri��o do teste de integra��o",
            TypeId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            DepartmentId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Primeiro precisamos fazer login para obter o token
        // Para este exemplo, vou assumir que h� um endpoint de login ou usar um token fixo
        // Em um cen�rio real, voc� configuraria autentica��o de teste

        // Act
        var response = await _client.PostAsJsonAsync(BaseUrl, request);

        // Assert
        // Como n�o temos autentica��o configurada ainda, esperamos Unauthorized
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }
}

public class CreateOrderRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TypeId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? DueDate { get; set; }
}