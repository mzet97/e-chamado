using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class DeletesDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<DeletesDepartmentCommandHandler> logger) :
    RequestHandlerAsync<DeletesDepartmentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeletesDepartmentCommand> HandleAsync(DeletesDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            logger.LogError("DeleteDepartmentCommand is null");
            throw new ArgumentNullException(nameof(command));
        }

        await unitOfWork.BeginTransactionAsync();

        foreach (var id in command.Ids)
        {
            var entity = await unitOfWork
                .Departments
                .GetByIdAsync(id);

            if (entity == null)
            {
                logger.LogError("Department not found");
                throw new NotFoundException("Department not found");
            }

            await unitOfWork.Departments
                .RemoveAsync(id);

            await unitOfWork.CommitAsync();

            await commandProcessor.PublishAsync(
                new DeletedDepartmentNotification(
                    entity.Id,
                    entity.Name,
                    entity.Description), cancellationToken: cancellationToken);
        }

        command.Result = new BaseResult(true, "Deletado com sucesso");
        return await base.HandleAsync(command, cancellationToken);
    }
}
