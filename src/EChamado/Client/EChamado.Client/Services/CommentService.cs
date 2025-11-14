using EChamado.Client.Models;
using EChamado.Shared.Responses;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

public class CommentService
{
    private readonly HttpClient _httpClient;

    public CommentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Busca todos os comentários de um chamado
    /// </summary>
    public async Task<List<CommentResponse>> GetByOrderIdAsync(Guid orderId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResultList<CommentResponse>>($"v1/order/{orderId}/comments");
            return result?.Data?.ToList() ?? new List<CommentResponse>();
        }
        catch (HttpRequestException)
        {
            return new List<CommentResponse>();
        }
    }

    /// <summary>
    /// Cria um novo comentário em um chamado
    /// </summary>
    public async Task<Guid> CreateAsync(Guid orderId, CreateCommentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"v1/order/{orderId}/comments", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Deleta um comentário
    /// </summary>
    public async Task DeleteAsync(Guid commentId)
    {
        var response = await _httpClient.DeleteAsync($"v1/comments/{commentId}");
        response.EnsureSuccessStatusCode();
    }
}
