using EChamado.Server.Application.UseCases.StatusTypes.Notifications;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.StatusTypes.Notifications.Handlers;

public class CreatedStatusTypeNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<CreatedStatusTypeNotificationHandler> logger) :
    RequestHandlerAsync<CreatedStatusTypeNotification>
{
    public override async Task<CreatedStatusTypeNotification> HandleAsync(
        CreatedStatusTypeNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("CreatedStatusTypeNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "status-type.created",
            "status-type-exchange",
            "direct",
            "create-status-type");

        logger.LogInformation("CreatedStatusTypeNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class UpdatedStatusTypeNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<UpdatedStatusTypeNotificationHandler> logger) :
    RequestHandlerAsync<UpdatedStatusTypeNotification>
{
    public override async Task<UpdatedStatusTypeNotification> HandleAsync(
        UpdatedStatusTypeNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("UpdatedStatusTypeNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "status-type.updated",
            "status-type-exchange",
            "direct",
            "update-status-type");

        logger.LogInformation("UpdatedStatusTypeNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DisabledStatusTypeNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DisabledStatusTypeNotificationHandler> logger) :
    RequestHandlerAsync<DisabledStatusTypeNotification>
{
    public override async Task<DisabledStatusTypeNotification> HandleAsync(
        DisabledStatusTypeNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DisabledStatusTypeNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "status-type.disabled",
            "status-type-exchange",
            "direct",
            "disable-status-type");

        logger.LogInformation("DisabledStatusTypeNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DeletedStatusTypeNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DeletedStatusTypeNotificationHandler> logger) :
    RequestHandlerAsync<DeletedStatusTypeNotification>
{
    public override async Task<DeletedStatusTypeNotification> HandleAsync(
        DeletedStatusTypeNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DeletedStatusTypeNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "status-type.deleted",
            "status-type-exchange",
            "direct",
            "delete-status-type");

        logger.LogInformation("DeletedStatusTypeNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}
