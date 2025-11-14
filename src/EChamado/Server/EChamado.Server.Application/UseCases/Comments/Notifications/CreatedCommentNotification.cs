using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Comments.Notifications;

public class CreatedCommentNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public string Text { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;

    public CreatedCommentNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public CreatedCommentNotification(Guid id, string text, Guid orderId, Guid userId, string userEmail)
    {
        Id = new Id(id.ToString());
        Text = text;
        OrderId = orderId;
        UserId = userId;
        UserEmail = userEmail;
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
