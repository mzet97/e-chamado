using EChamado.Client.Models;
using EChamado.Shared.Responses;
using System.Net.Http.Json;

namespace EChamado.Client.Services;

public class OrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Cria um novo chamado
    /// </summary>
    public async Task<Guid> CreateAsync(CreateOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/orders", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Atualiza um chamado existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"v1/orders/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Fecha um chamado com avaliação
    /// </summary>
    public async Task CloseAsync(Guid id, int evaluation)
    {
        var response = await _httpClient.PostAsJsonAsync($"v1/orders/{id}/close", new CloseOrderRequest(evaluation));
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Atribui um chamado a um responsável
    /// </summary>
    public async Task AssignAsync(Guid id, AssignOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"v1/orders/{id}/assign", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Altera o status de um chamado
    /// </summary>
    public async Task ChangeStatusAsync(Guid id, ChangeStatusRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"v1/orders/{id}/status", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Adiciona um comentário a um chamado
    /// </summary>
    public async Task<Guid> AddCommentAsync(Guid id, AddCommentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"v1/orders/{id}/comments", request);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<BaseResult<Guid>>();
        return result?.Data ?? Guid.Empty;
    }

    /// <summary>
    /// Busca um chamado por ID
    /// </summary>
    public async Task<OrderViewModel?> GetByIdAsync(Guid id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<BaseResult<OrderViewModel>>($"v1/orders/{id}");
            return result?.Data;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    /// <summary>
    /// Busca chamados com filtros e paginação
    /// </summary>
    public async Task<PagedResult<OrderListViewModel>> SearchAsync(SearchOrdersParameters parameters)
    {
        var queryString = BuildQueryString(parameters);
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<OrderListViewModel>>($"v1/orders?{queryString}");
        
        if (result?.Data != null)
        {
            var pagedData = result.PagedResult;
            return new PagedResult<OrderListViewModel>(
                result.Data.ToList(),
                pagedData?.RowCount ?? 0,
                pagedData?.CurrentPage ?? 1,
                pagedData?.PageSize ?? 10,
                pagedData?.PageCount ?? 0
            );
        }
        
        return new PagedResult<OrderListViewModel>(new List<OrderListViewModel>(), 0, 1, 10, 0);
    }

    /// <summary>
    /// Busca chamados do usuário logado
    /// </summary>
    public async Task<PagedResult<OrderListViewModel>> GetMyTicketsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<OrderListViewModel>>(
            $"v1/orders/my-tickets?pageNumber={pageNumber}&pageSize={pageSize}");
            
        if (result?.Data != null)
        {
            var pagedData = result.PagedResult;
            return new PagedResult<OrderListViewModel>(
                result.Data.ToList(),
                pagedData?.RowCount ?? 0,
                pagedData?.CurrentPage ?? 1,
                pagedData?.PageSize ?? 10,
                pagedData?.PageCount ?? 0
            );
        }
        
        return new PagedResult<OrderListViewModel>(new List<OrderListViewModel>(), 0, 1, 10, 0);
    }

    /// <summary>
    /// Busca chamados atribuídos ao usuário logado
    /// </summary>
    public async Task<PagedResult<OrderListViewModel>> GetAssignedToMeAsync(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _httpClient.GetFromJsonAsync<BaseResultList<OrderListViewModel>>(
            $"v1/orders/assigned-to-me?pageNumber={pageNumber}&pageSize={pageSize}");
            
        if (result?.Data != null)
        {
            var pagedData = result.PagedResult;
            return new PagedResult<OrderListViewModel>(
                result.Data.ToList(),
                pagedData?.RowCount ?? 0,
                pagedData?.CurrentPage ?? 1,
                pagedData?.PageSize ?? 10,
                pagedData?.PageCount ?? 0
            );
        }
        
        return new PagedResult<OrderListViewModel>(new List<OrderListViewModel>(), 0, 1, 10, 0);
    }

    private static string BuildQueryString(SearchOrdersParameters parameters)
    {
        var queryParams = new List<string>
        {
            $"pageNumber={parameters.PageNumber}",
            $"pageSize={parameters.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(parameters.SearchText))
            queryParams.Add($"searchText={Uri.EscapeDataString(parameters.SearchText)}");

        if (parameters.StatusId.HasValue)
            queryParams.Add($"statusId={parameters.StatusId.Value}");

        if (parameters.TypeId.HasValue)
            queryParams.Add($"typeId={parameters.TypeId.Value}");

        if (parameters.DepartmentId.HasValue)
            queryParams.Add($"departmentId={parameters.DepartmentId.Value}");

        if (parameters.CategoryId.HasValue)
            queryParams.Add($"categoryId={parameters.CategoryId.Value}");

        if (parameters.RequestingUserId.HasValue)
            queryParams.Add($"requestingUserId={parameters.RequestingUserId.Value}");

        if (parameters.ResponsibleUserId.HasValue)
            queryParams.Add($"responsibleUserId={parameters.ResponsibleUserId.Value}");

        if (parameters.StartDate.HasValue)
            queryParams.Add($"startDate={parameters.StartDate.Value:yyyy-MM-dd}");

        if (parameters.EndDate.HasValue)
            queryParams.Add($"endDate={parameters.EndDate.Value:yyyy-MM-dd}");

        if (parameters.IsOverdue.HasValue)
            queryParams.Add($"isOverdue={parameters.IsOverdue.Value}");

        return string.Join("&", queryParams);
    }
}

/// <summary>
/// Request para adicionar comentário
/// </summary>
public record AddCommentRequest(string Text, Guid UserId, string UserEmail);

/// <summary>
/// Request para alterar status
/// </summary>
public record ChangeStatusRequest(Guid StatusId);

/// <summary>
/// Request para atribuir chamado
/// </summary>
public record AssignOrderRequest(Guid AssignedToUserId);

/// <summary>
/// Request para fechar chamado
/// </summary>
public record CloseOrderRequest(int Evaluation);

/// <summary>
/// Parâmetros para busca de chamados
/// </summary>
public class SearchOrdersParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
    public Guid? StatusId { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? RequestingUserId { get; set; }
    public Guid? ResponsibleUserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsOverdue { get; set; }
}
