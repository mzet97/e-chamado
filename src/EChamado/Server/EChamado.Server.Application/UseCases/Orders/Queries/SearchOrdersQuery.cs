using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class SearchOrdersQuery : IRequest<BaseResultList<OrderListViewModel>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
    public Guid? StatusId { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? RequestingUserId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsOverdue { get; set; }
}
