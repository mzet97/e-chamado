using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;

public class OrderTypeUpdated : DomainEvent
{
    public OrderType OrderType { get; }

    public OrderTypeUpdated(OrderType orderType)
    {
        OrderType = orderType;
    }
}
