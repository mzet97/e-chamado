using EChamado.Server.Domain.Domains.Orders.Events.Orders;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class OrderUpdatedBrighterEvent : Event
{
    public OrderUpdated DomainEvent { get; }

    public OrderUpdatedBrighterEvent(OrderUpdated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
