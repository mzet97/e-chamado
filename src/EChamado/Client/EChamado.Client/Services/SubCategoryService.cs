using EChamado.Client.Models;
using EChamado.Shared.Responses;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

public class SubCategoryService
{
    private readonly HttpClient _httpClient;

    public SubCategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Busca todas as subcategorias com paginação
    /// </summary>
    public async Task<List<SubCategoryResponse>> GetAllAsync(int pageSize = 100)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<SubCategoryResponse>>($"v1/subcategories?PageSize={pageSize}");
        return result?.Data?.ToList() ?? new List<SubCategoryResponse>();
    }

    /// <summary>
    /// Busca subcategorias por categoria
    /// </summary>
    public async Task<List<SubCategoryResponse>> GetByCategoryIdAsync(Guid categoryId)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<SubCategoryResponse>>($"v1/subcategories?CategoryId={categoryId}");
        return result?.Data?.ToList() ?? new List<SubCategoryResponse>();
    }

    /// <summary>
    /// Busca uma subcategoria por ID
    /// </summary>
    public async Task<SubCategoryResponse?> GetByIdAsync(Guid id)
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

    /// <summary>
    /// Cria uma nova subcategoria
    /// </summary>
    public async Task<Guid> CreateAsync(CreateSubCategoryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/subcategories", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza uma subcategoria existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateSubCategoryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/subcategories/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta uma subcategoria
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/subcategories/{id}");
        response.EnsureSuccessStatusCode();
    }
}