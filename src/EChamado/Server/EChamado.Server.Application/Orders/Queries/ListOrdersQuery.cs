using EChamado.Server.Application.UseCases.Orders.ViewModels;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.Queries;

public sealed class ListOrdersQuery : IQuery<IEnumerable<OrderListViewModel>>
{
    public Guid? StatusId { get; init; }
    public Guid? TypeId { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? DepartmentId { get; init; }
    public Guid? RequestingUserId { get; init; }
    public Guid? ResponsibleUserId { get; init; }
}
