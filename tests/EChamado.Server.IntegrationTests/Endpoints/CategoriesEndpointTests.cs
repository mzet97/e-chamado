using System.Net;
using System.Net.Http.Json;
using EChamado.Server.IntegrationTests.Infrastructure;
using EChamado.Shared.Responses;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.IntegrationTests.Endpoints;

public class CategoriesEndpointTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebAppFactory _factory;

    public CategoriesEndpointTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCategory_ValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new { Name = "Categoria Teste", Description = "Descrição teste" };

        // Act
        var response = await _client.PostAsJsonAsync("/v1/category", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetCategoryById_ExistingCategory_ShouldReturnCategory()
    {
        // Arrange - Primeiro cria uma categoria
        var createRequest = new { Name = "Categoria GetById", Description = "Descrição" };
        var createResponse = await _client.PostAsJsonAsync("/v1/category", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        var categoryId = createResult!.Data;

        // Act
        var response = await _client.GetAsync($"/v1/category/{categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<BaseResult<CategoryResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(categoryId);
        result.Data.Name.Should().Be("Categoria GetById");
    }

    [Fact]
    public async Task SearchCategories_ShouldReturnList()
    {
        // Arrange - Cria algumas categorias
        await _client.PostAsJsonAsync("/v1/category", new { Name = "Cat 1", Description = "Desc 1" });
        await _client.PostAsJsonAsync("/v1/category", new { Name = "Cat 2", Description = "Desc 2" });

        // Act
        var response = await _client.GetAsync("/v1/categories?PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<BaseResultList<CategoryResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data.Count().Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task UpdateCategory_ValidRequest_ShouldUpdateCategory()
    {
        // Arrange - Cria uma categoria
        var createRequest = new { Name = "Categoria Original", Description = "Desc Original" };
        var createResponse = await _client.PostAsJsonAsync("/v1/category", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        var categoryId = createResult!.Data;

        var updateRequest = new { Name = "Categoria Atualizada", Description = "Desc Atualizada" };

        // Act
        var response = await _client.PutAsJsonAsync($"/v1/category/{categoryId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verifica se foi atualizada
        var getResponse = await _client.GetAsync($"/v1/category/{categoryId}");
        var getResult = await getResponse.Content.ReadFromJsonAsync<BaseResult<CategoryResponse>>();
        getResult!.Data!.Name.Should().Be("Categoria Atualizada");
    }

    [Fact]
    public async Task DeleteCategory_ExistingCategory_ShouldDeleteCategory()
    {
        // Arrange - Cria uma categoria
        var createRequest = new { Name = "Categoria para Deletar", Description = "Desc" };
        var createResponse = await _client.PostAsJsonAsync("/v1/category", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        var categoryId = createResult!.Data;

        // Act
        var response = await _client.DeleteAsync($"/v1/category/{categoryId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verifica se foi deletada (soft delete)
        var getResponse = await _client.GetAsync($"/v1/category/{categoryId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public record CategoryResponse(Guid Id, string Name, string Description, List<SubCategoryResponse> SubCategories);
public record SubCategoryResponse(Guid Id, string Name, string Description, Guid CategoryId);
