using EChamado.Server.Application.UseCases.SubCategories.Notifications;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.SubCategories.Notifications.Handlers;

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

public class DisabledSubCategoryNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DisabledSubCategoryNotificationHandler> logger) :
    RequestHandlerAsync<DisabledSubCategoryNotification>
{
    public override async Task<DisabledSubCategoryNotification> HandleAsync(
        DisabledSubCategoryNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DisabledSubCategoryNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "sub-category.disabled",
            "sub-category-exchange",
            "direct",
            "disable-sub-category");

        logger.LogInformation("DisabledSubCategoryNotification: {Notification}", notification);

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
