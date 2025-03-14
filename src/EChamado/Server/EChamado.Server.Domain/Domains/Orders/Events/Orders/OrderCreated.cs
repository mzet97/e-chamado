using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Orders;

public class OrderCreated : DomainEvent
{
    public Order Order { get; }

    public OrderCreated(Order order)
    {
        Order = order;
    }
}
