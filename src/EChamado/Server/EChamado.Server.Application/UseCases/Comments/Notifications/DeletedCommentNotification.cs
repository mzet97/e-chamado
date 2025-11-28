using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Comments.Notifications;

public class DeletedCommentNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public Guid OrderId { get; set; }

    public DeletedCommentNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public DeletedCommentNotification(Guid id, Guid orderId)
    {
        Id = new Id(id.ToString());
        OrderId = orderId;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
