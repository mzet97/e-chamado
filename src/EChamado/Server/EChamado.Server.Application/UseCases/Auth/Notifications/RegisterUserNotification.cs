using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Auth.Notifications;

public class RegisterUserNotification : IRequest
{
    public Id Id { get; set; }
    public Id CorrelationId { get; set; } = new Id(Guid.NewGuid().ToString());
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public RegisterUserNotification()
    {
        Id = new Id(Guid.NewGuid().ToString());
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
