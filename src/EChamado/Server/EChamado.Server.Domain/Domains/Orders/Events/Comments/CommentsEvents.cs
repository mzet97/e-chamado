using EChamado.Shared.Domain;

namespace EChamado.Server.Domain.Domains.Orders.Events.Comments;

public sealed record CommentCreated(
    Guid CommentId,
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    string Text
) : DomainEvent;

public sealed record CommentDeleted(
    Guid CommentId,
    Guid OrderId,
    Guid UserId,
    string UserEmail
) : DomainEvent;