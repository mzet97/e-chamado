using EChamado.Client.Models;
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
    /// Busca todos os departamentos
    /// </summary>
    public async Task<List<DepartmentResponse>> GetAllAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<DepartmentResponse>>("api/departments");
        return result ?? new List<DepartmentResponse>();
    }

    /// <summary>
    /// Busca um departamento por ID
    /// </summary>
    public async Task<DepartmentResponse?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DepartmentResponse>($"api/departments/{id}");
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
        var response = await _httpClient.PostAsJsonAsync("api/departments", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    /// <summary>
    /// Atualiza um departamento existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateDepartmentRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/departments/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta um departamento
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/departments/{id}");
        response.EnsureSuccessStatusCode();
    }
}
