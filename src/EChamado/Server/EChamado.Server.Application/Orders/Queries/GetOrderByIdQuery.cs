using EChamado.Server.Application.UseCases.Orders.ViewModels;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.Queries;

public sealed class GetOrderByIdQuery : IQuery<OrderViewModel?>
{
    public Guid OrderId { get; }

    public GetOrderByIdQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
