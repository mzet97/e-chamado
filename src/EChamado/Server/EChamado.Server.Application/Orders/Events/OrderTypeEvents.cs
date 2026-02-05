using EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class OrderTypeCreatedBrighterEvent : Event
{
    public OrderTypeCreated DomainEvent { get; }

    public OrderTypeCreatedBrighterEvent(OrderTypeCreated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}

public sealed class OrderTypeUpdatedBrighterEvent : Event
{
    public OrderTypeUpdated DomainEvent { get; }

    public OrderTypeUpdatedBrighterEvent(OrderTypeUpdated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
