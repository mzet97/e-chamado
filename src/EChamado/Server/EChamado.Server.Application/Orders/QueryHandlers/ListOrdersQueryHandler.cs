using EChamado.Server.Application.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Services;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.QueryHandlers;

public sealed class ListOrdersQueryHandler : QueryHandlerAsync<ListOrdersQuery, IEnumerable<OrderListViewModel>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ListOrdersQueryHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public override async Task<IEnumerable<OrderListViewModel>> ExecuteAsync(ListOrdersQuery query, CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.Orders.FindAsync(order =>
            (!query.StatusId.HasValue || order.StatusId == query.StatusId) &&
            (!query.TypeId.HasValue || order.TypeId == query.TypeId) &&
            (!query.CategoryId.HasValue || order.CategoryId == query.CategoryId) &&
            (!query.DepartmentId.HasValue || order.DepartmentId == query.DepartmentId) &&
            (!query.RequestingUserId.HasValue || order.RequestingUserId == query.RequestingUserId) &&
            (!query.ResponsibleUserId.HasValue || order.ResponsibleUserId == query.ResponsibleUserId));

        var statusById = (await _unitOfWork.StatusTypes.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var typeById = (await _unitOfWork.OrderTypes.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var departmentById = (await _unitOfWork.Departments.GetAllAsync()).ToDictionary(x => x.Id, x => x.Name);
        var utcNow = _dateTimeProvider.UtcNow;

        return orders.Select(order => new OrderListViewModel(
            order.Id,
            order.Title,
            order.OpeningDate ?? utcNow,
            order.ClosingDate,
            order.DueDate,
            statusById.GetValueOrDefault(order.StatusId, string.Empty),
            typeById.GetValueOrDefault(order.TypeId, string.Empty),
            departmentById.GetValueOrDefault(order.DepartmentId),
            order.RequestingUserEmail,
            order.ResponsibleUserEmail,
            order.DueDate.HasValue && order.DueDate.Value < utcNow && !order.ClosingDate.HasValue));
    }
}
