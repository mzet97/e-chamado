using EChamado.Client.Models;
using EChamado.Shared.Responses;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Busca todos os usuários
    /// </summary>
    public async Task<List<UserResponse>> GetAllAsync(int pageSize = 100)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<UserResponse>>($"v1/users?PageSize={pageSize}");
        return result?.Data?.ToList() ?? new List<UserResponse>();
    }

    /// <summary>
    /// Busca um usuário por ID
    /// </summary>
    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<UserResponse>>($"v1/users/{id}");
            return result?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Busca um usuário por email
    /// </summary>
    public async Task<UserResponse?> GetByEmailAsync(string email)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<UserResponse>>($"v1/users/email/{email}");
            return result?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    public async Task<Guid> CreateAsync(CreateUserRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/users", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/users/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deleta um usuário
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"v1/users/{id}");
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Altera senha de um usuário
    /// </summary>
    public async Task ChangePasswordAsync(Guid id, ChangePasswordRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/users/{id}/password", request);
        response.EnsureSuccessStatusCode();
    }
}