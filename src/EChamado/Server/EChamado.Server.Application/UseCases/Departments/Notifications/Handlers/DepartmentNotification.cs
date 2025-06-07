using EChamado.Server.Infrastructure.MessageBus;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Departments.Notifications.Handlers;

public class DepartmentNotification(
    IMessageBusClient messageBusClient,
    ILogger<DepartmentNotification> logger) :
    INotificationHandler<CreatedDepartmentNotification>,
    INotificationHandler<UpdatedDepartmentNotification>,
    INotificationHandler<DeletedDepartmentNotification>,
    INotificationHandler<DisabledDepartmentNotification>
{
    public async Task Handle(CreatedDepartmentNotification notification, CancellationToken cancellationToken)
{
    if (notification == null)
    {
        logger.LogError("CreatedDepartmentNotification is null");
        return;
    }

    await messageBusClient.Publish(notification.ToString(),
        "departament.created",
        "departament-exchange",
        "direct",
        "create-departament");

    logger.LogInformation("CreatedDepartmentNotification: " + notification);
}

public async Task Handle(UpdatedDepartmentNotification notification, CancellationToken cancellationToken)
{
    if (notification == null)
    {
        logger.LogError("UpdatedDepartmentNotification is null");
        return;
    }

    await messageBusClient.Publish(notification.ToString(),
        "departament.updated",
        "departament-exchange",
        "direct",
        "update-departament");

    logger.LogInformation("UpdatedDepartmentNotification: " + notification);
}

public async Task Handle(DisabledDepartmentNotification notification, CancellationToken cancellationToken)
{
    if (notification == null)
    {
        logger.LogError("DisabledDepartmentNotification is null");
        return;
    }

    await messageBusClient.Publish(notification.ToString(),
        "departament.disabled",
        "departament-exchange",
        "direct",
        "disable-departament");

    logger.LogInformation("DisabledDepartmentNotification: " + notification);
}

public async Task Handle(DeletedDepartmentNotification notification, CancellationToken cancellationToken)
{
    if (notification == null)
    {
        logger.LogError("DeletedDepartmentNotification is null");
        return;
    }

    await messageBusClient.Publish(notification.ToString(),
        "departament.deleted",
        "departament-exchange",
        "direct",
        "delete-departament");

    logger.LogInformation("DeletedDepartmentNotification: " + notification);
}
}
