using EChamado.Server.Domain.Domains.Orders.Events.Orders;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class OrderCreatedBrighterEvent : Event
{
    public OrderCreated DomainEvent { get; }

    public OrderCreatedBrighterEvent(OrderCreated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
