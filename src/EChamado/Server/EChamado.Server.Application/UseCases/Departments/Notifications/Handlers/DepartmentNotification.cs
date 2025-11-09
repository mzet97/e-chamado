using EChamado.Server.Infrastructure.MessageBus;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Departments.Notifications.Handlers;

public class CreatedDepartmentNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<CreatedDepartmentNotificationHandler> logger) : RequestHandlerAsync<CreatedDepartmentNotification>
{
    public override async Task<CreatedDepartmentNotification> HandleAsync(CreatedDepartmentNotification notification, CancellationToken cancellationToken = default)
    {
        if(notification == null)
        {
            logger.LogError("CreatedDepartmentNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(notification.ToString(),
            "departament.created",
            "departament-exchange",
            "direct",
            "create-departament");

        logger.LogInformation("CreatedDepartmentNotification: " + notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class UpdatedDepartmentNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<UpdatedDepartmentNotificationHandler> logger) : RequestHandlerAsync<UpdatedDepartmentNotification>
{
    public override async Task<UpdatedDepartmentNotification> HandleAsync(UpdatedDepartmentNotification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("UpdatedDepartmentNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(notification.ToString(),
            "departament.updated",
            "departament-exchange",
            "direct",
            "update-departament");

        logger.LogInformation("UpdatedDepartmentNotification: " + notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DisabledDepartmentNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DisabledDepartmentNotificationHandler> logger) : RequestHandlerAsync<DisabledDepartmentNotification>
{
    public override async Task<DisabledDepartmentNotification> HandleAsync(DisabledDepartmentNotification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DisabledDepartmentNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(notification.ToString(),
            "departament.disabled",
            "departament-exchange",
            "direct",
            "disable-departament");

        logger.LogInformation("DisabledDepartmentNotification: " + notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}

public class DeletedDepartmentNotificationHandler(
    IMessageBusClient messageBusClient,
    ILogger<DeletedDepartmentNotificationHandler> logger) : RequestHandlerAsync<DeletedDepartmentNotification>
{
    public override async Task<DeletedDepartmentNotification> HandleAsync(DeletedDepartmentNotification notification, CancellationToken cancellationToken = default)
    {
        if (notification == null)
        {
            logger.LogError("DeletedDepartmentNotification is null");
            return await base.HandleAsync(notification, cancellationToken);
        }

        await messageBusClient.Publish(notification.ToString(),
            "departament.deleted",
            "departament-exchange",
            "direct",
            "delete-departament");

        logger.LogInformation("DeletedDepartmentNotification: " + notification);

        return await base.HandleAsync(notification, cancellationToken);
    }
}
