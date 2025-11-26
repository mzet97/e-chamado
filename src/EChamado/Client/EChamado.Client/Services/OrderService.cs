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
    public async Task UpdateAsync(UpdateOrderRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync("v1/orders", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Fecha um chamado com avaliação
    /// </summary>
    public async Task CloseAsync(Guid id, int? evaluation = null)
    {
        var payload = new CloseOrderRequest(id, evaluation);
        var response = await _httpClient.PostAsJsonAsync("v1/orders/close", payload);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Atribui um chamado a um responsável
    /// </summary>
    public async Task AssignAsync(Guid orderId, Guid assignedToUserId)
    {
        var payload = new AssignOrderRequest(orderId, assignedToUserId);
        var response = await _httpClient.PostAsJsonAsync("v1/orders/assign", payload);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Altera o status de um chamado
    /// </summary>
    public async Task ChangeStatusAsync(Guid orderId, Guid statusTypeId)
    {
        var payload = new ChangeStatusRequest(orderId, statusTypeId);
        var response = await _httpClient.PostAsJsonAsync("v1/orders/status", payload);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Adiciona um comentário a um chamado
    /// </summary>
    public async Task<Guid> AddCommentAsync(AddCommentRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("v1/comments", request);
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
        var response = await _httpClient.GetAsync($"v1/orders?{queryString}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            var reason = string.IsNullOrWhiteSpace(error) ? response.ReasonPhrase : error;
            throw new HttpRequestException(reason, null, response.StatusCode);
        }

        var result = await response.Content.ReadFromJsonAsync<BaseResultList<OrderListViewModel>>();

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
    public Task<PagedResult<OrderListViewModel>> GetMyTicketsAsync(Guid userId, int pageIndex = 1, int pageSize = 10)
        => SearchAsync(new SearchOrdersParameters
        {
            CreatedByUserId = userId,
            PageIndex = pageIndex,
            PageSize = pageSize
        });

    /// <summary>
    /// Busca chamados atribuídos ao usuário logado
    /// </summary>
    public Task<PagedResult<OrderListViewModel>> GetAssignedToMeAsync(Guid userId, int pageIndex = 1, int pageSize = 10)
        => SearchAsync(new SearchOrdersParameters
        {
            AssignedToUserId = userId,
            PageIndex = pageIndex,
            PageSize = pageSize
        });

    private static string BuildQueryString(SearchOrdersParameters parameters)
    {
        var queryParams = new List<string>
        {
            $"pageIndex={parameters.PageIndex}",
            $"pageSize={parameters.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(parameters.Title))
            queryParams.Add($"title={Uri.EscapeDataString(parameters.Title)}");

        if (!string.IsNullOrWhiteSpace(parameters.Description))
            queryParams.Add($"description={Uri.EscapeDataString(parameters.Description)}");

        if (parameters.StatusTypeId.HasValue)
            queryParams.Add($"statusTypeId={parameters.StatusTypeId.Value}");

        if (parameters.TypeId.HasValue)
            queryParams.Add($"typeId={parameters.TypeId.Value}");

        if (parameters.DepartmentId.HasValue)
            queryParams.Add($"departmentId={parameters.DepartmentId.Value}");

        if (parameters.StartDate.HasValue)
            queryParams.Add($"startDate={Uri.EscapeDataString(parameters.StartDate.Value.ToString("o"))}");

        if (parameters.EndDate.HasValue)
            queryParams.Add($"endDate={Uri.EscapeDataString(parameters.EndDate.Value.ToString("o"))}");

        if (parameters.CategoryId.HasValue)
            queryParams.Add($"categoryId={parameters.CategoryId.Value}");

        if (parameters.SubCategoryId.HasValue)
            queryParams.Add($"subCategoryId={parameters.SubCategoryId.Value}");

        if (parameters.CreatedByUserId.HasValue)
            queryParams.Add($"createdByUserId={parameters.CreatedByUserId.Value}");

        if (parameters.AssignedToUserId.HasValue)
            queryParams.Add($"assignedToUserId={parameters.AssignedToUserId.Value}");

        if (parameters.IsOverdue.HasValue)
            queryParams.Add($"isOverdue={parameters.IsOverdue.Value}");

        return string.Join("&", queryParams);
    }
}

/// <summary>
/// Parâmetros para busca de chamados
/// </summary>
public class SearchOrdersParameters
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? StatusTypeId { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public bool? IsOverdue { get; set; }
}
