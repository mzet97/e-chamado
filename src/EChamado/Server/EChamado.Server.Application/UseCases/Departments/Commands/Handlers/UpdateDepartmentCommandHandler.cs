using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<UpdateDepartmentCommandHandler> logger) :
    RequestHandlerAsync<UpdateDepartmentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateDepartmentCommand> HandleAsync(
        UpdateDepartmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var entityDb = await unitOfWork
            .Departments
            .GetByIdAsync(command.Id);

        if(entityDb == null)
        {
            logger.LogError("Department not found");
            throw new NotFoundException("Department not found");
        }

        entityDb.Update(command.Name, command.Description, dateTimeProvider);

        if (!entityDb.IsValid())
        {
            logger.LogError("Validate Department has error");
            throw new ValidationException(
                "Validate Department has error",
                entityDb.GetErrors());
        }

        await unitOfWork
            .Departments
            .UpdateAsync(entityDb);

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(
            new UpdatedDepartmentNotification(
            entityDb.Id,
            entityDb.Name,
            entityDb.Description), cancellationToken: cancellationToken);

        command.Result = new BaseResult(true, "Atualizado com sucesso");
        return await base.HandleAsync(command, cancellationToken);
    }
}
