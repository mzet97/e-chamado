using EChamado.Client.Models;
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
        var response = await _httpClient.PostAsJsonAsync("api/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    /// <summary>
    /// Atualiza um chamado existente
    /// </summary>
    public async Task UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Fecha um chamado com avaliação
    /// </summary>
    public async Task CloseAsync(Guid id, int evaluation)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/orders/{id}/close", new CloseOrderRequest(evaluation));
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Atribui um chamado a um responsável
    /// </summary>
    public async Task AssignAsync(Guid id, Guid responsibleUserId, string responsibleUserEmail)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/orders/{id}/assign",
            new AssignOrderRequest(responsibleUserId, responsibleUserEmail));
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Busca um chamado por ID
    /// </summary>
    public async Task<OrderViewModel?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<OrderViewModel>($"api/orders/{id}");
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
        var result = await _httpClient.GetFromJsonAsync<PagedResult<OrderListViewModel>>($"api/orders?{queryString}");
        return result ?? new PagedResult<OrderListViewModel>(new List<OrderListViewModel>(), 0, 1, 10, 0);
    }

    /// <summary>
    /// Busca chamados do usuário logado
    /// </summary>
    public async Task<PagedResult<OrderListViewModel>> GetMyTicketsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _httpClient.GetFromJsonAsync<PagedResult<OrderListViewModel>>(
            $"api/orders/my-tickets?pageNumber={pageNumber}&pageSize={pageSize}");
        return result ?? new PagedResult<OrderListViewModel>(new List<OrderListViewModel>(), 0, 1, 10, 0);
    }

    /// <summary>
    /// Busca chamados atribuídos ao usuário logado
    /// </summary>
    public async Task<PagedResult<OrderListViewModel>> GetAssignedToMeAsync(int pageNumber = 1, int pageSize = 10)
    {
        var result = await _httpClient.GetFromJsonAsync<PagedResult<OrderListViewModel>>(
            $"api/orders/assigned-to-me?pageNumber={pageNumber}&pageSize={pageSize}");
        return result ?? new PagedResult<OrderListViewModel>(new List<OrderListViewModel>(), 0, 1, 10, 0);
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
