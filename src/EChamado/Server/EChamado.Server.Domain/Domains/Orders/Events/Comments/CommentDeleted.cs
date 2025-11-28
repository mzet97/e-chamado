using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Comments;

public class CommentDeleted : DomainEvent
{
    public Comment Comment { get; }

    public CommentDeleted(Comment comment)
    {
        Comment = comment;
    }
}
