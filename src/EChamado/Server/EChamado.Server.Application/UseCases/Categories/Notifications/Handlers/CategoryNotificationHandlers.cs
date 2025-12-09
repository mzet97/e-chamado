using EChamado.Server.Application.UseCases.Categories.Notifications;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Categories.Notifications.Handlers;

public class CreatedCategoryNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<CreatedCategoryNotificationHandler> logger) :
    RequestHandlerAsync<CreatedCategoryNotification>
{
    public override async Task<CreatedCategoryNotification> HandleAsync(
        CreatedCategoryNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("CreatedCategoryNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "category.created",
            "category-exchange",
            "direct",
            "create-category");

        logger.LogInformation("CreatedCategoryNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class UpdatedCategoryNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<UpdatedCategoryNotificationHandler> logger) :
    RequestHandlerAsync<UpdatedCategoryNotification>
{
    public override async Task<UpdatedCategoryNotification> HandleAsync(
        UpdatedCategoryNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("UpdatedCategoryNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "category.updated",
            "category-exchange",
            "direct",
            "update-category");

        logger.LogInformation("UpdatedCategoryNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DeletedCategoryNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DeletedCategoryNotificationHandler> logger) :
    RequestHandlerAsync<DeletedCategoryNotification>
{
    public override async Task<DeletedCategoryNotification> HandleAsync(
        DeletedCategoryNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DeletedCategoryNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "category.deleted",
            "category-exchange",
            "direct",
            "delete-category");

        logger.LogInformation("DeletedCategoryNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class CreatedSubCategoryNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<CreatedSubCategoryNotificationHandler> logger) :
    RequestHandlerAsync<CreatedSubCategoryNotification>
{
    public override async Task<CreatedSubCategoryNotification> HandleAsync(
        CreatedSubCategoryNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("CreatedSubCategoryNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "sub-category.created",
            "sub-category-exchange",
            "direct",
            "create-sub-category");

        logger.LogInformation("CreatedSubCategoryNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class UpdatedSubCategoryNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<UpdatedSubCategoryNotificationHandler> logger) :
    RequestHandlerAsync<UpdatedSubCategoryNotification>
{
    public override async Task<UpdatedSubCategoryNotification> HandleAsync(
        UpdatedSubCategoryNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("UpdatedSubCategoryNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "sub-category.updated",
            "sub-category-exchange",
            "direct",
            "update-sub-category");

        logger.LogInformation("UpdatedSubCategoryNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DeletedSubCategoryNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DeletedSubCategoryNotificationHandler> logger) :
    RequestHandlerAsync<DeletedSubCategoryNotification>
{
    public override async Task<DeletedSubCategoryNotification> HandleAsync(
        DeletedSubCategoryNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DeletedSubCategoryNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "sub-category.deleted",
            "sub-category-exchange",
            "direct",
            "delete-sub-category");

        logger.LogInformation("DeletedSubCategoryNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}
