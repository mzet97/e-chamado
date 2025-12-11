using EChamado.Server.Domain.Domains.Orders.Events.Comments;
using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Events;

public sealed class CommentCreatedBrighterEvent : Event
{
    public CommentCreated DomainEvent { get; }

    public CommentCreatedBrighterEvent(CommentCreated domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}

public sealed class CommentDeletedBrighterEvent : Event
{
    public CommentDeleted DomainEvent { get; }

    public CommentDeletedBrighterEvent(CommentDeleted domainEvent) : base(domainEvent.EventId)
    {
        DomainEvent = domainEvent;
    }
}
