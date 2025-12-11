using EChamado.Server.Application.UseCases.Comments.Notifications;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Notifications.Handlers;

public class CreatedCommentNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<CreatedCommentNotificationHandler> logger) :
    RequestHandlerAsync<CreatedCommentNotification>
{
    public override async Task<CreatedCommentNotification> HandleAsync(
        CreatedCommentNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("CreatedCommentNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "comment.created",
            "comment-exchange",
            "direct",
            "create-comment");

        logger.LogInformation("CreatedCommentNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DeletedCommentNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DeletedCommentNotificationHandler> logger) :
    RequestHandlerAsync<DeletedCommentNotification>
{
    public override async Task<DeletedCommentNotification> HandleAsync(
        DeletedCommentNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DeletedCommentNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "comment.deleted",
            "comment-exchange",
            "direct",
            "delete-comment");

        logger.LogInformation("DeletedCommentNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DisabledCommentNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DisabledCommentNotificationHandler> logger) :
    RequestHandlerAsync<DisabledCommentNotification>
{
    public override async Task<DisabledCommentNotification> HandleAsync(
        DisabledCommentNotification notification,
        CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DisabledCommentNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(
            notification.ToString(),
            "comment.disabled",
            "comment-exchange",
            "direct",
            "disable-comment");

        logger.LogInformation("DisabledCommentNotification: {Notification}", notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}
