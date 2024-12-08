using MediatR;
using System.Text.Json;

namespace EChamado.Application.Features.Auth.Notifications;

public class LoginUserNotification : INotification
{
    public string Email { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public override string? ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
