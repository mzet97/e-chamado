using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;


public class OrderTypeCreated : DomainEvent
{
    public OrderType OrderType { get; }

    public OrderTypeCreated(OrderType orderType)
    {
        OrderType = orderType;
    }
}
