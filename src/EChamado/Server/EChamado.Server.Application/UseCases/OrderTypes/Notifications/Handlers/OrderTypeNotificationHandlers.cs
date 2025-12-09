using EChamado.Server.Application.UseCases.OrderTypes.Notifications;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.OrderTypes.Notifications.Handlers;

public class CreatedOrderTypeNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<CreatedOrderTypeNotificationHandler> logger) :
    RequestHandlerAsync<CreatedOrderTypeNotification>
{
    public override async Task<CreatedOrderTypeNotification> HandleAsync(
        CreatedOrderTypeNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("CreatedOrderTypeNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "order-type.created",
            "order-type-exchange",
            "direct",
            "create-order-type");

        logger.LogInformation("CreatedOrderTypeNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class UpdatedOrderTypeNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<UpdatedOrderTypeNotificationHandler> logger) :
    RequestHandlerAsync<UpdatedOrderTypeNotification>
{
    public override async Task<UpdatedOrderTypeNotification> HandleAsync(
        UpdatedOrderTypeNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("UpdatedOrderTypeNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "order-type.updated",
            "order-type-exchange",
            "direct",
            "update-order-type");

        logger.LogInformation("UpdatedOrderTypeNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DeletedOrderTypeNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DeletedOrderTypeNotificationHandler> logger) :
    RequestHandlerAsync<DeletedOrderTypeNotification>
{
    public override async Task<DeletedOrderTypeNotification> HandleAsync(
        DeletedOrderTypeNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DeletedOrderTypeNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "order-type.deleted",
            "order-type-exchange",
            "direct",
            "delete-order-type");

        logger.LogInformation("DeletedOrderTypeNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}
