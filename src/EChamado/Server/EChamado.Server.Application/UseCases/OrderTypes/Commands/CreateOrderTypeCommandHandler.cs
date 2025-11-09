using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.OrderTypes.Notifications;
using EChamado.Server.Domain.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class CreateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<CreateOrderTypeCommandHandler> logger) :
    RequestHandlerAsync<CreateOrderTypeCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateOrderTypeCommand> HandleAsync(CreateOrderTypeCommand command, CancellationToken cancellationToken = default)
    {
        var entity = OrderType.Create(command.Name, command.Description);

        if (!entity.IsValid())
        {
            logger.LogError("Validate OrderType has error");
            throw new ValidationException("Validate OrderType has error", entity.GetErrors());
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
