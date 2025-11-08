using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class SearchOrdersQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<SearchOrdersQueryHandler> logger) :
    IRequestHandler<SearchOrdersQuery, BaseResultList<OrderListViewModel>>
{
    public async Task<BaseResultList<OrderListViewModel>> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        // Busca todos os orders
        var query = await unitOfWork.Orders.GetAllAsync(cancellationToken);

        // Aplica filtros
        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchLower = request.SearchText.ToLower();
            filtered = filtered.Where(o =>
                o.Title.ToLower().Contains(searchLower) ||
                o.Description.ToLower().Contains(searchLower));
        }

        if (request.StatusId.HasValue)
            filtered = filtered.Where(o => o.StatusId == request.StatusId.Value);

        if (request.TypeId.HasValue)
            filtered = filtered.Where(o => o.TypeId == request.TypeId.Value);

        if (request.DepartmentId.HasValue)
            filtered = filtered.Where(o => o.DepartmentId == request.DepartmentId.Value);

        if (request.CategoryId.HasValue)
            filtered = filtered.Where(o => o.CategoryId == request.CategoryId.Value);

        if (request.RequestingUserId.HasValue)
            filtered = filtered.Where(o => o.RequestingUserId == request.RequestingUserId.Value);

        if (request.AssignedToUserId.HasValue)
            filtered = filtered.Where(o => o.ResponsibleUserId == request.AssignedToUserId.Value);

        if (request.StartDate.HasValue)
            filtered = filtered.Where(o => o.OpeningDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            filtered = filtered.Where(o => o.OpeningDate <= request.EndDate.Value);

        if (request.IsOverdue.HasValue && request.IsOverdue.Value)
            filtered = filtered.Where(o => o.DueDate.HasValue && o.DueDate.Value < DateTime.UtcNow && !o.ClosingDate.HasValue);

        var totalCount = filtered.Count();

        // Paginação
        var orders = filtered
            .OrderByDescending(o => o.OpeningDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Busca dados relacionados
        var statuses = await unitOfWork.StatusTypes.GetAllAsync(cancellationToken);
        var types = await unitOfWork.OrderTypes.GetAllAsync(cancellationToken);
        var departments = await unitOfWork.Departments.GetAllAsync(cancellationToken);

        var statusDict = statuses.ToDictionary(s => s.Id, s => s.Name);
        var typeDict = types.ToDictionary(t => t.Id, t => t.Name);
        var departmentDict = departments.ToDictionary(d => d.Id, d => d.Name);

        // Mapeia para ViewModels
        var items = orders.Select(o => new OrderListViewModel(
            o.Id,
            o.Title,
            o.OpeningDate,
            o.ClosingDate,
            o.DueDate,
            statusDict.GetValueOrDefault(o.StatusId, "Unknown"),
            typeDict.GetValueOrDefault(o.TypeId, "Unknown"),
            o.DepartmentId.HasValue ? departmentDict.GetValueOrDefault(o.DepartmentId.Value) : null,
            o.RequestingUserEmail,
            o.ResponsibleUserEmail,
            o.DueDate.HasValue && o.DueDate.Value < DateTime.UtcNow && !o.ClosingDate.HasValue
        )).ToList();

        logger.LogInformation("Search orders returned {Count} results", items.Count);

        return new BaseResultList<OrderListViewModel>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
