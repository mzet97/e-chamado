using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Orders;

public class OrderClosed : DomainEvent
{
    public Order Order { get; }

    public OrderClosed(Order order)
    {
        Order = order;
    }
}
