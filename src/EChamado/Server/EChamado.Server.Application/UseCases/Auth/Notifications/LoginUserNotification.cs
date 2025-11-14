using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Auth.Notifications;

public class LoginUserNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public LoginUserNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
        CorrelationId = new Id(Guid.NewGuid().ToString());
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
