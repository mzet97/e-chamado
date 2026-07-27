using EChamado.Server.Domain.Domains.Orders.Events.Categories;
using EChamado.Server.Domain.Domains.Orders.Events.Comments;
using EChamado.Server.Domain.Domains.Orders.Events.Departments;
using EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;
using EChamado.Server.Domain.Domains.Orders.Events.Orders;
using EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;
using EChamado.Server.Domain.Domains.Orders.Events.SubCategories;
using Paramore.Brighter;

namespace EChamado.Server.Application.Events;

// Integração (Brighter) events que mapeiam Domain Events para o barramento.
// Anteriormente em Application/Orders/Events/ (legacy), consolidados aqui.

// -------- Orders --------
public sealed class OrderCreatedBrighterEvent : Event
{
    public OrderCreated DomainEvent { get; }
    public OrderCreatedBrighterEvent(OrderCreated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class OrderUpdatedBrighterEvent : Event
{
    public OrderUpdated DomainEvent { get; }
    public OrderUpdatedBrighterEvent(OrderUpdated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class OrderClosedBrighterEvent : Event
{
    public OrderClosed DomainEvent { get; }
    public OrderClosedBrighterEvent(OrderClosed domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

// -------- Categories --------
public sealed class CategoryCreatedBrighterEvent : Event
{
    public CategoryCreated DomainEvent { get; }
    public CategoryCreatedBrighterEvent(CategoryCreated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class CategoryUpdatedBrighterEvent : Event
{
    public CategoryUpdated DomainEvent { get; }
    public CategoryUpdatedBrighterEvent(CategoryUpdated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

// -------- SubCategories --------
public sealed class SubCategoryCreatedBrighterEvent : Event
{
    public SubCategoryCreated DomainEvent { get; }
    public SubCategoryCreatedBrighterEvent(SubCategoryCreated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class SubCategoryUpdatedBrighterEvent : Event
{
    public SubCategoryUpdated DomainEvent { get; }
    public SubCategoryUpdatedBrighterEvent(SubCategoryUpdated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

// -------- Departments --------
public sealed class DepartmentCreatedBrighterEvent : Event
{
    public DepartmentCreated DomainEvent { get; }
    public DepartmentCreatedBrighterEvent(DepartmentCreated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class DepartmentUpdatedBrighterEvent : Event
{
    public DepartmentUpdated DomainEvent { get; }
    public DepartmentUpdatedBrighterEvent(DepartmentUpdated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

// -------- OrderTypes --------
public sealed class OrderTypeCreatedBrighterEvent : Event
{
    public OrderTypeCreated DomainEvent { get; }
    public OrderTypeCreatedBrighterEvent(OrderTypeCreated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class OrderTypeUpdatedBrighterEvent : Event
{
    public OrderTypeUpdated DomainEvent { get; }
    public OrderTypeUpdatedBrighterEvent(OrderTypeUpdated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

// -------- StatusTypes --------
public sealed class StatusTypeCreatedBrighterEvent : Event
{
    public StatusTypeCreated DomainEvent { get; }
    public StatusTypeCreatedBrighterEvent(StatusTypeCreated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class StatusTypeUpdatedBrighterEvent : Event
{
    public StatusTypeUpdated DomainEvent { get; }
    public StatusTypeUpdatedBrighterEvent(StatusTypeUpdated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

// -------- Comments --------
public sealed class CommentCreatedBrighterEvent : Event
{
    public CommentCreated DomainEvent { get; }
    public CommentCreatedBrighterEvent(CommentCreated domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}

public sealed class CommentDeletedBrighterEvent : Event
{
    public CommentDeleted DomainEvent { get; }
    public CommentDeletedBrighterEvent(CommentDeleted domainEvent) : base(domainEvent.EventId)
        => DomainEvent = domainEvent;
}
