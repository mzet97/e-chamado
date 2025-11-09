using EChamado.Client.Models;
using EChamado.Shared.Responses;
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
    /// Busca todos os tipos de chamado (com cache) usando Search
    /// </summary>
    public async Task<List<OrderTypeResponse>> GetOrderTypesAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedOrderTypes != null)
            return _cachedOrderTypes;

        var result = await _httpClient.GetFromJsonAsync<BaseResultList<OrderTypeResponse>>("v1/ordertypes?PageSize=1000");
        _cachedOrderTypes = result?.Data?.ToList() ?? new List<OrderTypeResponse>();
        return _cachedOrderTypes;
    }

    /// <summary>
    /// Busca todos os status (com cache) usando Search
    /// </summary>
    public async Task<List<StatusTypeResponse>> GetStatusTypesAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedStatusTypes != null)
            return _cachedStatusTypes;

        var result = await _httpClient.GetFromJsonAsync<BaseResultList<StatusTypeResponse>>("v1/statustypes?PageSize=1000");
        _cachedStatusTypes = result?.Data?.ToList() ?? new List<StatusTypeResponse>();
        return _cachedStatusTypes;
    }

    /// <summary>
    /// Busca todos os departamentos (com cache) usando Search
    /// </summary>
    public async Task<List<DepartmentResponse>> GetDepartmentsAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedDepartments != null)
            return _cachedDepartments;

        var result = await _httpClient.GetFromJsonAsync<BaseResultList<DepartmentResponse>>("v1/departments?PageSize=1000");
        _cachedDepartments = result?.Data?.ToList() ?? new List<DepartmentResponse>();
        return _cachedDepartments;
    }

    /// <summary>
    /// Busca todas as categorias (com cache) usando Search
    /// </summary>
    public async Task<List<CategoryResponse>> GetCategoriesAsync(bool forceRefresh = false)
    {
        if (!forceRefresh && _cachedCategories != null)
            return _cachedCategories;

        var result = await _httpClient.GetFromJsonAsync<BaseResultList<CategoryResponse>>("v1/categories?PageSize=1000");
        _cachedCategories = result?.Data?.ToList() ?? new List<CategoryResponse>();
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
        var response = await _httpClient.PostAsJsonAsync("v1/ordertype", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza um tipo de chamado
    /// </summary>
    public async Task UpdateOrderTypeAsync(Guid id, UpdateOrderTypeRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/ordertype/{id}", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
    }

    /// <summary>
    /// Deleta um tipo de chamado
    /// </summary>
    public async Task DeleteOrderTypeAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/ordertype/{id}");
        response.EnsureSuccessStatusCode();
        ClearCache();
    }

    /// <summary>
    /// Cria um novo status
    /// </summary>
    public async Task<Guid> CreateStatusTypeAsync(CreateStatusTypeRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/statustype", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza um status
    /// </summary>
    public async Task UpdateStatusTypeAsync(Guid id, UpdateStatusTypeRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/statustype/{id}", request);
        response.EnsureSuccessStatusCode();
        ClearCache();
    }

    /// <summary>
    /// Deleta um status
    /// </summary>
    public async Task DeleteStatusTypeAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/statustype/{id}");
        response.EnsureSuccessStatusCode();
        ClearCache();
    }
}
