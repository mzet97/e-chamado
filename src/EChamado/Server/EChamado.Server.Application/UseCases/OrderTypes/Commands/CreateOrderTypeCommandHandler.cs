using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.OrderTypes.Notifications;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class CreateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateOrderTypeCommandHandler> logger) :
    RequestHandlerAsync<CreateOrderTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateOrderTypeCommand> HandleAsync(CreateOrderTypeCommand command, CancellationToken cancellationToken = default)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var entity = OrderType.Create(command.Name, command.Description, dateTimeProvider);

        if (!entity.IsValid())
        {
            logger.LogError("Validate OrderType has error");
            throw new ValidationException("Validate OrderType has error", entity.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.OrderTypes.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new CreatedOrderTypeNotification(entity.Id, entity.Name, entity.Description), cancellationToken: cancellationToken);

        logger.LogInformation("OrderType {OrderTypeId} created successfully", entity.Id);

        command.Result = new BaseResult<Guid>(entity.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}

