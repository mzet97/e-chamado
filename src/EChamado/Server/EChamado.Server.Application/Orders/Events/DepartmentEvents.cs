using EChamado.Server.Domain.Domains.Orders.Events.Departments;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class DepartmentCreatedBrighterEvent : Event
{
    public DepartmentCreated DomainEvent { get; }

    public DepartmentCreatedBrighterEvent(DepartmentCreated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}

public sealed class DepartmentUpdatedBrighterEvent : Event
{
    public DepartmentUpdated DomainEvent { get; }

    public DepartmentUpdatedBrighterEvent(DepartmentUpdated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
