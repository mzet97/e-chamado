using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class DisableDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DisableDepartmentCommandHandler> logger) :
    RequestHandlerAsync<DisableDepartmentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DisableDepartmentCommand> HandleAsync(DisableDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DisableDepartmentCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        var entity = await unitOfWork
            .Departments
            .GetByIdAsync(command.Id);

        if (entity == null)
        {
            logger.LogError("Department not found");
            throw new NotFoundException("Department not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Departments
            .DisableAsync(command.Id);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(
            new DeletedDepartmentNotification(
                entity.Id,
                entity.Name,
                entity.Description), cancellationToken: cancellationToken);

        command.Result = new BaseResult(true, "Desativado com sucesso");
        return await base.HandleAsync(command, cancellationToken);
    }
}
