using EChamado.Server.Application.UseCases.Orders.ViewModels;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public record SearchOrdersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchText = null,
    Guid? StatusId = null,
    Guid? TypeId = null,
    Guid? DepartmentId = null,
    Guid? CategoryId = null,
    Guid? RequestingUserId = null,
    Guid? ResponsibleUserId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    bool? IsOverdue = null
) : IRequest<PagedResult<OrderListViewModel>>;
