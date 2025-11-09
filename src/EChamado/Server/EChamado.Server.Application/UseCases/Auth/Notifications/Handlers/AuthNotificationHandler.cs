using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Auth.Notifications.Handlers;

public class LoginUserNotificationHandler(ILogger<LoginUserNotificationHandler> logger) : RequestHandlerAsync<LoginUserNotification>
{
    public override async Task<LoginUserNotification> HandleAsync(LoginUserNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("LoginUserNotification: " + notification.Message);
        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class RegisterUserNotificationHandler(ILogger<RegisterUserNotificationHandler> logger) : RequestHandlerAsync<RegisterUserNotification>
{
    public override async Task<RegisterUserNotification> HandleAsync(RegisterUserNotification notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("RegisterUserNotification: " + notification.Message);
        return await base.HandleAsync(notification, cancellationToken);
    }
}
