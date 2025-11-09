using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Comments.Notifications;

public class DeletedCommentNotification : IRequest
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }

    public DeletedCommentNotification()
    {
        Id = Guid.NewGuid();
    }

    public DeletedCommentNotification(Guid id, Guid orderId)
    {
        Id = id;
        OrderId = orderId;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
