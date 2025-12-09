using EChamado.Server.Domain.Domains.Orders.Events.SubCategories;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class SubCategoryCreatedBrighterEvent : Event
{
    public SubCategoryCreated DomainEvent { get; }

    public SubCategoryCreatedBrighterEvent(SubCategoryCreated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}

public sealed class SubCategoryUpdatedBrighterEvent : Event
{
    public SubCategoryUpdated DomainEvent { get; }

    public SubCategoryUpdatedBrighterEvent(SubCategoryUpdated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
