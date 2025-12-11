using EChamado.Server.Domain.Domains.Orders.Events.Categories;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class CategoryCreatedBrighterEvent : Event
{
    public CategoryCreated DomainEvent { get; }

    public CategoryCreatedBrighterEvent(CategoryCreated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}

public sealed class CategoryUpdatedBrighterEvent : Event
{
    public CategoryUpdated DomainEvent { get; }

    public CategoryUpdatedBrighterEvent(CategoryUpdated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
