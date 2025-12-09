using EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class StatusTypeCreatedBrighterEvent : Event
{
    public StatusTypeCreated DomainEvent { get; }

    public StatusTypeCreatedBrighterEvent(StatusTypeCreated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}

public sealed class StatusTypeUpdatedBrighterEvent : Event
{
    public StatusTypeUpdated DomainEvent { get; }

    public StatusTypeUpdatedBrighterEvent(StatusTypeUpdated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
