using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Application.Features.Auth.Notifications.Handlers;

public class AuthNotificationHandler(ILogger<AuthNotificationHandler> logger) :
    INotificationHandler<LoginUserNotification>,
    INotificationHandler<RegisterUserNotification>
{

    public Task Handle(LoginUserNotification notification, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            logger.LogInformation("LoginUserNotification: " + notification.Message);
        });
    }

    public Task Handle(RegisterUserNotification notification, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            logger.LogInformation("RegisterUserNotification: " + notification.Message);
        });
    }
}
