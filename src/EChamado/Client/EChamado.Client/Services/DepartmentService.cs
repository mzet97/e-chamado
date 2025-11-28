using EChamado.Client.Models;
using EChamado.Shared.Responses;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

public class DepartmentService
{
    private readonly HttpClient _httpClient;

    public DepartmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Busca todos os departamentos usando Search
    /// </summary>
    public async Task<List<DepartmentResponse>> GetAllAsync(int pageSize = 100)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<DepartmentResponse>>($"v1/departments?PageSize={pageSize}");
        return result?.Data?.ToList() ?? new List<DepartmentResponse>();
    }

    /// <summary>
    /// Busca um departamento por ID
    /// </summary>
    public async Task<DepartmentResponse?> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<DepartmentResponse>>($"v1/departments/{id}");
            return result?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Cria um novo departamento
    /// </summary>
    public async Task<Guid> CreateAsync(CreateDepartmentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/departments", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza um departamento existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateDepartmentRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/departments/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta um departamento
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/departments/{id}");
        response.EnsureSuccessStatusCode();
    }
}
