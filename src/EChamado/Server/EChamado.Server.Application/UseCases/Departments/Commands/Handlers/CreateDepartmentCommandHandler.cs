using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Departments.Notifications;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Departments.Commands.Handlers;

public class CreateDepartmentCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateDepartmentCommandHandler> logger) :
    RequestHandlerAsync<CreateDepartmentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateDepartmentCommand> HandleAsync(CreateDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        var entity = Department.Create(command.Name, command.Description, dateTimeProvider);

        if (!entity.IsValid())
        {
            logger.LogError("Validate Department has error");
            throw new ValidationException("Validate Department has error", entity.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Departments.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new CreatedDepartmentNotification(entity.Id, entity.Name, entity.Description), cancellationToken: cancellationToken);

        command.Result = new BaseResult<Guid>(entity.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}

