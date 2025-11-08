using EChamado.Client.Models;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Busca todas as categorias com suas subcategorias
    /// </summary>
    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<CategoryResponse>>("api/categories");
        return result ?? new List<CategoryResponse>();
    }

    /// <summary>
    /// Busca uma categoria por ID
    /// </summary>
    public async Task<CategoryResponse?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CategoryResponse>($"api/categories/{id}");
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    public async Task<Guid> CreateAsync(CreateCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/categories", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta uma categoria
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{id}");
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Cria uma nova subcategoria
    /// </summary>
    public async Task<Guid> CreateSubCategoryAsync(Guid categoryId, CreateSubCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/categories/{categoryId}/subcategories", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    /// <summary>
    /// Atualiza uma subcategoria
    /// </summary>
    public async Task UpdateSubCategoryAsync(Guid id, UpdateSubCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/categories/subcategories/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta uma subcategoria
    /// </summary>
    public async Task DeleteSubCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/subcategories/{id}");
        response.EnsureSuccessStatusCode();
    }
}
