using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Comments.Notifications;

public class DeletedCommentNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public DeletedCommentNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public DeletedCommentNotification(Guid id, Guid orderId, Guid userId, string userEmail, string text)
    {
        Id = new Id(id.ToString());
        OrderId = orderId;
        UserId = userId;
        UserEmail = userEmail;
        Text = text;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
