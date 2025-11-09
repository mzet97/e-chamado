using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Comments;

public class CommentCreated : DomainEvent
{
    public Comment Comment { get; }

    public CommentCreated(Comment comment)
    {
        Comment = comment;
        AggregateId = comment.OrderId;
    }
}
