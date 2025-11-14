using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class SearchOrdersQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<SearchOrdersQueryHandler> logger) :
    RequestHandlerAsync<SearchOrdersQuery>
{
    public override async Task<SearchOrdersQuery> HandleAsync(SearchOrdersQuery query, CancellationToken cancellationToken = default)
    {
        // Busca todos os orders
        var orders = await unitOfWork.Orders.GetAllAsync();

        // Aplica filtros
        var filtered = orders.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var searchLower = query.SearchText.ToLower();
            filtered = filtered.Where(o =>
                o.Title.ToLower().Contains(searchLower) ||
                o.Description.ToLower().Contains(searchLower));
        }

        if (query.StatusId.HasValue)
            filtered = filtered.Where(o => o.StatusId == query.StatusId.Value);

        if (query.TypeId.HasValue)
            filtered = filtered.Where(o => o.TypeId == query.TypeId.Value);

        if (query.DepartmentId.HasValue)
            filtered = filtered.Where(o => o.DepartmentId == query.DepartmentId.Value);

        if (query.CategoryId.HasValue)
            filtered = filtered.Where(o => o.CategoryId == query.CategoryId.Value);

        if (query.RequestingUserId.HasValue)
            filtered = filtered.Where(o => o.RequestingUserId == query.RequestingUserId.Value);

        if (query.AssignedToUserId.HasValue)
            filtered = filtered.Where(o => o.ResponsibleUserId == query.AssignedToUserId.Value);

        if (query.StartDate.HasValue)
            filtered = filtered.Where(o => o.OpeningDate.HasValue && o.OpeningDate.Value >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            filtered = filtered.Where(o => o.OpeningDate.HasValue && o.OpeningDate.Value <= query.EndDate.Value);

        if (query.IsOverdue.HasValue && query.IsOverdue.Value)
            filtered = filtered.Where(o => o.DueDate.HasValue && o.DueDate.Value < DateTime.UtcNow && !o.ClosingDate.HasValue);

        var totalCount = filtered.Count();

        // Paginação
        var ordersResult = filtered
            .OrderByDescending(o => o.OpeningDate)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        // Busca dados relacionados
        var statuses = await unitOfWork.StatusTypes.GetAllAsync();
        var types = await unitOfWork.OrderTypes.GetAllAsync();
        var departments = await unitOfWork.Departments.GetAllAsync();

        var statusDict = statuses.ToDictionary(s => s.Id, s => s.Name);
        var typeDict = types.ToDictionary(t => t.Id, t => t.Name);
        var departmentDict = departments.ToDictionary(d => d.Id, d => d.Name);

        // Mapeia para ViewModels
        var items = ordersResult.Select(o => new OrderListViewModel(
            o.Id,
            o.Title,
            o.OpeningDate ?? DateTime.UtcNow,
            o.ClosingDate,
            o.DueDate,
            statusDict.GetValueOrDefault(o.StatusId, "Unknown"),
            typeDict.GetValueOrDefault(o.TypeId, "Unknown"),
            departmentDict.GetValueOrDefault(o.DepartmentId),
            o.RequestingUserEmail,
            o.ResponsibleUserEmail,
            o.DueDate.HasValue && o.DueDate.Value < DateTime.UtcNow && !o.ClosingDate.HasValue
        )).ToList();

        logger.LogInformation("Search orders returned {Count} results", items.Count);

        var pagedResult = PagedResult.Create(query.PageNumber, query.PageSize, totalCount);
        query.Result = new BaseResultList<OrderListViewModel>(items, pagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
