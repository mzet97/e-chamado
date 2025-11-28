using EChamado.Client.Models;
using EChamado.Shared.Responses;
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
    /// Busca todas as categorias com suas subcategorias usando Search
    /// </summary>
    public async Task<List<CategoryResponse>> GetAllAsync(int pageSize = 100)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<CategoryResponse>>($"v1/categories?PageSize={pageSize}");
        return result?.Data?.ToList() ?? new List<CategoryResponse>();
    }

    /// <summary>
    /// Busca uma categoria por ID
    /// </summary>
    public async Task<CategoryResponse?> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<CategoryResponse>>($"v1/categories/{id}");
            return result?.Data;
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
        var response = await _httpClient.PostAsJsonAsync("v1/categories", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/categories/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta uma categoria
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/categories/{id}");
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Cria uma nova subcategoria
    /// </summary>
    public async Task<Guid> CreateSubCategoryAsync(CreateSubCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/subcategories", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza uma subcategoria
    /// </summary>
    public async Task UpdateSubCategoryAsync(Guid id, UpdateSubCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/subcategories/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta uma subcategoria
    /// </summary>
    public async Task DeleteSubCategoryAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/subcategories/{id}");
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Busca todas as subcategorias usando Search
    /// </summary>
    public async Task<List<SubCategoryResponse>> GetAllSubCategoriesAsync(Guid? categoryId = null, int pageSize = 100)
    {
        var url = $"v1/subcategories?PageSize={pageSize}";
        if (categoryId.HasValue)
            url += $"&CategoryId={categoryId}";

        var result = await _httpClient.GetFromJsonAsync<BaseResultList<SubCategoryResponse>>(url);
        return result?.Data?.ToList() ?? new List<SubCategoryResponse>();
    }

    /// <summary>
    /// Busca uma subcategoria por ID
    /// </summary>
    public async Task<SubCategoryResponse?> GetSubCategoryByIdAsync(Guid id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<SubCategoryResponse>>($"v1/subcategories/{id}");
            return result?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}
