using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Repositories;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class SearchOrdersQueryHandler : IRequestHandler<SearchOrdersQuery, PagedResult<OrderListViewModel>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IStatusTypeRepository _statusTypeRepository;
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public SearchOrdersQueryHandler(
        IOrderRepository orderRepository,
        IStatusTypeRepository statusTypeRepository,
        IOrderTypeRepository orderTypeRepository,
        IDepartmentRepository departmentRepository)
    {
        _orderRepository = orderRepository;
        _statusTypeRepository = statusTypeRepository;
        _orderTypeRepository = orderTypeRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<PagedResult<OrderListViewModel>> Handle(SearchOrdersQuery request, CancellationToken cancellationToken)
    {
        // Busca todos os orders com filtros
        var query = await _orderRepository.GetAllAsync(cancellationToken);

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

        if (request.ResponsibleUserId.HasValue)
            filtered = filtered.Where(o => o.ResponsibleUserId == request.ResponsibleUserId.Value);

        if (request.StartDate.HasValue)
            filtered = filtered.Where(o => o.OpeningDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            filtered = filtered.Where(o => o.OpeningDate <= request.EndDate.Value);

        if (request.IsOverdue.HasValue && request.IsOverdue.Value)
            filtered = filtered.Where(o => o.DueDate.HasValue && o.DueDate.Value < DateTime.UtcNow && !o.ClosingDate.HasValue);

        var totalCount = filtered.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        // Paginação
        var orders = filtered
            .OrderByDescending(o => o.OpeningDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Busca dados relacionados
        var statusIds = orders.Select(o => o.StatusId).Distinct();
        var typeIds = orders.Select(o => o.TypeId).Distinct();
        var departmentIds = orders.Where(o => o.DepartmentId.HasValue).Select(o => o.DepartmentId!.Value).Distinct();

        var statuses = await _statusTypeRepository.GetAllAsync(cancellationToken);
        var types = await _orderTypeRepository.GetAllAsync(cancellationToken);
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);

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

        return new PagedResult<OrderListViewModel>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        );
    }
}
