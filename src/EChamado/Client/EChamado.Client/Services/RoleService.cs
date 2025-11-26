using EChamado.Client.Models;
using EChamado.Shared.Responses;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

public class RoleService
{
    private readonly HttpClient _httpClient;

    public RoleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Busca todos os perfis
    /// </summary>
    public async Task<List<RoleResponse>> GetAllAsync(int pageSize = 100)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<RoleResponse>>($"v1/role?PageSize={pageSize}");
        return result?.Data?.ToList() ?? new List<RoleResponse>();
    }

    /// <summary>
    /// Busca um perfil por ID
    /// </summary>
    public async Task<RoleResponse?> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<RoleResponse>>($"v1/role/{id}");
            return result?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Busca um perfil por nome
    /// </summary>
    public async Task<RoleResponse?> GetByNameAsync(string name)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<RoleResponse>>($"v1/role/name/{name}");
            return result?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Cria um novo perfil
    /// </summary>
    public async Task<Guid> CreateAsync(CreateRoleRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/role", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza um perfil existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateRoleRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/role/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta um perfil
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/role/{id}");
        response.EnsureSuccessStatusCode();
    }
}