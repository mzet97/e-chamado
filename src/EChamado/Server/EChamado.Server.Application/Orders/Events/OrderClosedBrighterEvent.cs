using EChamado.Server.Domain.Domains.Orders.Events.Orders;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class OrderClosedBrighterEvent : Event
{
    public OrderClosed DomainEvent { get; }

    public OrderClosedBrighterEvent(OrderClosed domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
