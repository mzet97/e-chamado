using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.StatusTypes.Notifications;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.StatusTypes.Commands;

public class CreateStatusTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateStatusTypeCommandHandler> logger) :
    RequestHandlerAsync<CreateStatusTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateStatusTypeCommand> HandleAsync(CreateStatusTypeCommand command, CancellationToken cancellationToken = default)
    {
        var entity = StatusType.Create(command.Name, command.Description, dateTimeProvider);

        if (!entity.IsValid())
        {
            logger.LogError("Validate StatusType has error");
            throw new ValidationException("Validate StatusType has error", entity.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.StatusTypes.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new CreatedStatusTypeNotification(entity.Id, entity.Name, entity.Description), cancellationToken: cancellationToken);

        logger.LogInformation("StatusType {StatusTypeId} created successfully", entity.Id);

        command.Result = new BaseResult<Guid>(entity.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}

