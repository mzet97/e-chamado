using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using LinqKit;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.Orders.Queries.Handlers;

public class SearchOrdersQueryHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    ILogger<SearchOrdersQueryHandler> logger) :
    RequestHandlerAsync<SearchOrdersQuery>
{
    public override async Task<SearchOrdersQuery> HandleAsync(SearchOrdersQuery query, CancellationToken cancellationToken = default)
    {
        // Constrói predicado para filtro no banco de dados (evita carregar todos na memória)
        Expression<Func<Order, bool>> filter = PredicateBuilder.New<Order>(true);

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var searchLower = query.SearchText.ToLower();
            filter = filter.And(o =>
                o.Title.ToLower().Contains(searchLower) ||
                o.Description.ToLower().Contains(searchLower));
        }

        if (query.StatusId.HasValue)
            filter = filter.And(o => o.StatusId == query.StatusId.Value);

        if (query.TypeId.HasValue)
            filter = filter.And(o => o.TypeId == query.TypeId.Value);

        if (query.DepartmentId.HasValue)
            filter = filter.And(o => o.DepartmentId == query.DepartmentId.Value);

        if (query.CategoryId.HasValue)
            filter = filter.And(o => o.CategoryId == query.CategoryId.Value);

        if (query.RequestingUserId.HasValue)
            filter = filter.And(o => o.RequestingUserId == query.RequestingUserId.Value);

        if (query.AssignedToUserId.HasValue)
            filter = filter.And(o => o.ResponsibleUserId == query.AssignedToUserId.Value);

        if (query.StartDate.HasValue)
            filter = filter.And(o => o.OpeningDate.HasValue && o.OpeningDate.Value >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            filter = filter.And(o => o.OpeningDate.HasValue && o.OpeningDate.Value <= query.EndDate.Value);

        var utcNow = dateTimeProvider.UtcNow;
        if (query.IsOverdue.HasValue && query.IsOverdue.Value)
            filter = filter.And(o => o.DueDate.HasValue && o.DueDate.Value < utcNow && !o.ClosingDate.HasValue);

        // Usa SearchAsync com paginação no banco de dados
        Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = q => q.OrderByDescending(o => o.OpeningDate);

        var result = await unitOfWork.Orders.SearchAsync(
            predicate: filter,
            orderBy: orderBy,
            pageSize: query.PageSize,
            page: query.PageNumber);

        // Busca dados relacionados para lookup (estes são pequenos, OK carregar todos)
        var statuses = await unitOfWork.StatusTypes.GetAllAsync();
        var types = await unitOfWork.OrderTypes.GetAllAsync();
        var departments = await unitOfWork.Departments.GetAllAsync();

        var statusDict = statuses.ToDictionary(s => s.Id, s => s.Name);
        var typeDict = types.ToDictionary(t => t.Id, t => t.Name);
        var departmentDict = departments.ToDictionary(d => d.Id, d => d.Name);

        // Mapeia para ViewModels
        var items = result.Data.Select(o => new OrderListViewModel(
            o.Id,
            o.Title,
            o.OpeningDate ?? utcNow,
            o.ClosingDate,
            o.DueDate,
            statusDict.GetValueOrDefault(o.StatusId, "Unknown"),
            typeDict.GetValueOrDefault(o.TypeId, "Unknown"),
            departmentDict.GetValueOrDefault(o.DepartmentId),
            o.RequestingUserEmail,
            o.ResponsibleUserEmail,
            o.DueDate.HasValue && o.DueDate.Value < utcNow && !o.ClosingDate.HasValue
        )).ToList();

        logger.LogInformation("Search orders returned {Count} results out of {Total}", items.Count, result.PagedResult?.RowCount ?? 0);

        query.Result = new BaseResultList<OrderListViewModel>(items, result.PagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
