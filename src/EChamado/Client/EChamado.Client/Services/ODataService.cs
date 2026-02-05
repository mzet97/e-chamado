using Simple.OData.Client;
using EChamado.Client.Models;
using Microsoft.Extensions.Configuration;

namespace EChamado.Client.Services;

public interface IODataService
{
    Task<IEnumerable<OrderListViewModel>> GetOrdersAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null);
    Task<OrderViewModel?> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(string? filter = null, string? orderBy = null, int? top = null);
    Task<IEnumerable<DepartmentResponse>> GetDepartmentsAsync(string? filter = null, string? orderBy = null, int? top = null);
    Task<IEnumerable<OrderTypeResponse>> GetOrderTypesAsync(string? filter = null, string? orderBy = null, int? top = null);
    Task<IEnumerable<StatusTypeResponse>> GetStatusTypesAsync(string? filter = null, string? orderBy = null, int? top = null);
    Task<IEnumerable<SubCategoryResponse>> GetSubCategoriesAsync(string? filter = null, string? orderBy = null, int? top = null);
    Task<int> GetOrdersCountAsync(string? filter = null);
}

public class ODataService : IODataService
{
    private readonly ODataClient _client;
    private readonly ILogger<ODataService> _logger;

    public ODataService(HttpClient httpClient, IConfiguration configuration, ILogger<ODataService> logger)
    {
        _logger = logger;

        // Simple.OData.Client expects a relative URI when using HttpClient with BaseAddress
        var settings = new ODataClientSettings(httpClient, new Uri("/odata", UriKind.Relative))
        {
            IgnoreResourceNotFoundException = true,
            OnTrace = (message, args) => _logger.LogDebug(message, args)
        };

        _client = new ODataClient(settings);
    }

    public async Task<IEnumerable<OrderListViewModel>> GetOrdersAsync(string? filter = null, string? orderBy = null, int? top = null, int? skip = null)
    {
        try
        {
            var query = _client.For("Orders")
                .Expand("Status,Type,Category,Department");

            if (!string.IsNullOrEmpty(filter))
                query = query.Filter(filter);

            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy);

            if (top.HasValue)
                query = query.Top(top.Value);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            var results = await query.FindEntriesAsync();

            return results.Select(r =>
            {
                var openingDate = r["OpeningDate"] as DateTime? ?? DateTime.Now;
                var closingDate = r["ClosingDate"] as DateTime?;
                var dueDate = r["DueDate"] as DateTime?;
                var isOverdue = dueDate.HasValue && dueDate.Value < DateTime.Now && !closingDate.HasValue;

                return new OrderListViewModel(
                    Id: r["Id"] as Guid? ?? Guid.Empty,
                    Title: r["Title"] as string ?? string.Empty,
                    OpeningDate: openingDate,
                    ClosingDate: closingDate,
                    DueDate: dueDate,
                    StatusName: GetExpandedProperty(r, "Status", "Name"),
                    TypeName: GetExpandedProperty(r, "Type", "Name"),
                    DepartmentName: GetExpandedProperty(r, "Department", "Name"),
                    RequestingUserEmail: r["RequestingUserEmail"] as string ?? string.Empty,
                    ResponsibleUserEmail: r["ResponsibleUserEmail"] as string,
                    IsOverdue: isOverdue
                );
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching orders via OData");
            throw;
        }
    }

    public async Task<OrderViewModel?> GetOrderByIdAsync(Guid id)
    {
        try
        {
            var result = await _client
                .For("Orders")
                .Key(id)
                .Expand("Status,Type,Category,SubCategory,Department")
                .FindEntryAsync();

            if (result == null)
                return null;

            return new OrderViewModel(
                Id: result["Id"] as Guid? ?? Guid.Empty,
                Title: result["Title"] as string ?? string.Empty,
                Description: result["Description"] as string ?? string.Empty,
                Evaluation: result["Evaluation"] as int?,
                OpeningDate: result["OpeningDate"] as DateTime? ?? DateTime.Now,
                ClosingDate: result["ClosingDate"] as DateTime?,
                DueDate: result["DueDate"] as DateTime?,
                StatusId: result["StatusId"] as Guid? ?? Guid.Empty,
                StatusName: GetExpandedProperty(result, "Status", "Name"),
                TypeId: result["TypeId"] as Guid? ?? Guid.Empty,
                TypeName: GetExpandedProperty(result, "Type", "Name"),
                CategoryId: result["CategoryId"] as Guid?,
                CategoryName: GetExpandedProperty(result, "Category", "Name"),
                SubCategoryId: result["SubCategoryId"] as Guid?,
                SubCategoryName: GetExpandedProperty(result, "SubCategory", "Name"),
                DepartmentId: result["DepartmentId"] as Guid?,
                DepartmentName: GetExpandedProperty(result, "Department", "Name"),
                RequestingUserId: result["RequestingUserId"] as Guid? ?? Guid.Empty,
                RequestingUserEmail: result["RequestingUserEmail"] as string ?? string.Empty,
                ResponsibleUserId: result["ResponsibleUserId"] as Guid?,
                ResponsibleUserEmail: result["ResponsibleUserEmail"] as string,
                CreatedAt: result["CreatedAt"] as DateTime? ?? DateTime.Now,
                UpdatedAt: result["UpdatedAt"] as DateTime?
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order {OrderId} via OData", id);
            return null;
        }
    }

    public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(string? filter = null, string? orderBy = null, int? top = null)
    {
        try
        {
            var query = _client.For("Categories");

            if (!string.IsNullOrEmpty(filter))
                query = query.Filter(filter);

            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy);

            if (top.HasValue)
                query = query.Top(top.Value);

            var results = await query.FindEntriesAsync();

            return results.Select(r => new CategoryResponse(
                Id: r["Id"] as Guid? ?? Guid.Empty,
                Name: r["Name"] as string ?? string.Empty,
                Description: r["Description"] as string ?? string.Empty,
                SubCategories: new List<SubCategoryResponse>()
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching categories via OData");
            throw;
        }
    }

    public async Task<IEnumerable<DepartmentResponse>> GetDepartmentsAsync(string? filter = null, string? orderBy = null, int? top = null)
    {
        try
        {
            var query = _client.For("Departments");

            if (!string.IsNullOrEmpty(filter))
                query = query.Filter(filter);

            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy);

            if (top.HasValue)
                query = query.Top(top.Value);

            var results = await query.FindEntriesAsync();

            return results.Select(r => new DepartmentResponse(
                Id: r["Id"] as Guid? ?? Guid.Empty,
                Name: r["Name"] as string ?? string.Empty,
                Description: r["Description"] as string ?? string.Empty
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching departments via OData");
            throw;
        }
    }

    public async Task<IEnumerable<OrderTypeResponse>> GetOrderTypesAsync(string? filter = null, string? orderBy = null, int? top = null)
    {
        try
        {
            var query = _client.For("OrderTypes");

            if (!string.IsNullOrEmpty(filter))
                query = query.Filter(filter);

            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy);

            if (top.HasValue)
                query = query.Top(top.Value);

            var results = await query.FindEntriesAsync();

            return results.Select(r => new OrderTypeResponse(
                Id: r["Id"] as Guid? ?? Guid.Empty,
                Name: r["Name"] as string ?? string.Empty,
                Description: r["Description"] as string ?? string.Empty
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order types via OData");
            throw;
        }
    }

    public async Task<IEnumerable<StatusTypeResponse>> GetStatusTypesAsync(string? filter = null, string? orderBy = null, int? top = null)
    {
        try
        {
            var query = _client.For("StatusTypes");

            if (!string.IsNullOrEmpty(filter))
                query = query.Filter(filter);

            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy);

            if (top.HasValue)
                query = query.Top(top.Value);

            var results = await query.FindEntriesAsync();

            return results.Select(r => new StatusTypeResponse(
                Id: r["Id"] as Guid? ?? Guid.Empty,
                Name: r["Name"] as string ?? string.Empty,
                Description: r["Description"] as string ?? string.Empty
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching status types via OData");
            throw;
        }
    }

    public async Task<IEnumerable<SubCategoryResponse>> GetSubCategoriesAsync(string? filter = null, string? orderBy = null, int? top = null)
    {
        try
        {
            var query = _client.For("SubCategories");

            if (!string.IsNullOrEmpty(filter))
                query = query.Filter(filter);

            if (!string.IsNullOrEmpty(orderBy))
                query = query.OrderBy(orderBy);

            if (top.HasValue)
                query = query.Top(top.Value);

            var results = await query.FindEntriesAsync();

            return results.Select(r => new SubCategoryResponse(
                id: r["Id"] as Guid? ?? Guid.Empty,
                name: r["Name"] as string ?? string.Empty,
                description: r["Description"] as string ?? string.Empty,
                categoryId: r["CategoryId"] as Guid? ?? Guid.Empty,
                categoryName: null
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sub-categories via OData");
            throw;
        }
    }

    public async Task<int> GetOrdersCountAsync(string? filter = null)
    {
        try
        {
            var query = _client.For("Orders");

            if (!string.IsNullOrEmpty(filter))
                query = query.Filter(filter);

            return await query.Count().FindScalarAsync<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting orders via OData");
            return 0;
        }
    }

    private static string GetExpandedProperty(IDictionary<string, object> entry, string navigationProperty, string propertyName)
    {
        if (entry.TryGetValue(navigationProperty, out var navProp) && navProp is IDictionary<string, object> dict)
        {
            if (dict.TryGetValue(propertyName, out var value))
                return value as string ?? string.Empty;
        }
        return string.Empty;
    }
}
