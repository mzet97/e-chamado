using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Orders;

public class OrderUpdated : DomainEvent
{
    public Order Order { get; }

    public OrderUpdated(Order order)
    {
        Order = order;
    }
}
