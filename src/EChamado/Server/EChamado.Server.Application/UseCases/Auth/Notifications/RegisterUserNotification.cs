using Paramore.Brighter;
using System.Text.Json;

namespace EChamado.Server.Application.UseCases.Auth.Notifications;

public class RegisterUserNotification : IRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public RegisterUserNotification()
    {
        Id = Guid.NewGuid();
    }

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
