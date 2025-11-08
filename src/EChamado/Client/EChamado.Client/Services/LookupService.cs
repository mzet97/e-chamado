using EChamado.Client.Models;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

/// <summary>
/// Serviço para buscar dados de lookup (dropdowns)
/// </summary>
public class LookupService
{
    private readonly HttpClient _httpClient;
    private List<OrderTypeResponse>? _cachedOrderTypes;
    private List<StatusTypeResponse>? _cachedStatusTypes;
    private List<DepartmentResponse>? _cachedDepartments;
    private List<CategoryResponse>? _cachedCategories;

    public LookupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Busca todos os tipos de chamado (com cache)
    /// </summary>
    public async Task<List<OrderTypeResponse>> GetOrderTypesAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedOrderTypes != null)
            return _cachedOrderTypes;

        var result = await _httpClient.GetFromJsonAsync<List<OrderTypeResponse>>("api/ordertypes");
        _cachedOrderTypes = result ?? new List<OrderTypeResponse>();
        return _cachedOrderTypes;
    }

    /// <summary>
    /// Busca todos os status (com cache)
    /// </summary>
    public async Task<List<StatusTypeResponse>> GetStatusTypesAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedStatusTypes != null)
            return _cachedStatusTypes;

        var result = await _httpClient.GetFromJsonAsync<List<StatusTypeResponse>>("api/statustypes");
        _cachedStatusTypes = result ?? new List<StatusTypeResponse>();
        return _cachedStatusTypes;
    }

    /// <summary>
    /// Busca todos os departamentos (com cache)
    /// </summary>
    public async Task<List<DepartmentResponse>> GetDepartmentsAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedDepartments != null)
            return _cachedDepartments;

        var result = await _httpClient.GetFromJsonAsync<List<DepartmentResponse>>("api/departments");
        _cachedDepartments = result ?? new List<DepartmentResponse>();
        return _cachedDepartments;
    }

    /// <summary>
    /// Busca todas as categorias (com cache)
    /// </summary>
    public async Task<List<CategoryResponse>> GetCategoriesAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedCategories != null)
            return _cachedCategories;

        var result = await _httpClient.GetFromJsonAsync<List<CategoryResponse>>("api/categories");
        _cachedCategories = result ?? new List<CategoryResponse>();
        return _cachedCategories;
    }

    /// <summary>
    /// Busca subcategorias de uma categoria específica
    /// </summary>
    public async Task<List<SubCategoryResponse>> GetSubCategoriesAsync(Guid categoryId)
    {
        var categories = await GetCategoriesAsync();
        var category = categories.FirstOrDefault(c => c.Id == categoryId);
        return category?.SubCategories ?? new List<SubCategoryResponse>();
    }

    /// <summary>
    /// Limpa todos os caches
    /// </summary>
    public void ClearCache()
    {
        _cachedOrderTypes = null;
        _cachedStatusTypes = null;
        _cachedDepartments = null;
        _cachedCategories = null;
    }

    /// <summary>
    /// Cria um novo tipo de chamado
    /// </summary>
    public async Task<Guid> CreateOrderTypeAsync(CreateOrderTypeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ordertypes", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    /// <summary>
    /// Atualiza um tipo de chamado
    /// </summary>
    public async Task UpdateOrderTypeAsync(Guid id, UpdateOrderTypeRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/ordertypes/{id}", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
    }

    /// <summary>
    /// Deleta um tipo de chamado
    /// </summary>
    public async Task DeleteOrderTypeAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/ordertypes/{id}");
        response.EnsureSuccessStatusCode();
        ClearCache();
    }

    /// <summary>
    /// Cria um novo status
    /// </summary>
    public async Task<Guid> CreateStatusTypeAsync(CreateStatusTypeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/statustypes", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    /// <summary>
    /// Atualiza um status
    /// </summary>
    public async Task UpdateStatusTypeAsync(Guid id, UpdateStatusTypeRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/statustypes/{id}", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
    }

    /// <summary>
    /// Deleta um status
    /// </summary>
    public async Task DeleteStatusTypeAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/statustypes/{id}");
        response.EnsureSuccessStatusCode();
        ClearCache();
    }
}
