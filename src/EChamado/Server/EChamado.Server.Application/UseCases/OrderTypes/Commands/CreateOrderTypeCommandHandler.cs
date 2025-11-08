using EChamado.Server.Domain.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

using EChamado.Server.Application.UseCases.OrderTypes.Notifications;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class CreateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<CreateOrderTypeCommandHandler> logger) :
    IRequestHandler<CreateOrderTypeCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateOrderTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = OrderType.Create(request.Name, request.Description);

        if (!entity.IsValid())
        {
            logger.LogError("Validate OrderType has error");
            throw new ValidationException("Validate OrderType has error", entity.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.OrderTypes.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new CreatedOrderTypeNotification(entity.Id, entity.Name, entity.Description));

        logger.LogInformation("OrderType {OrderTypeId} created successfully", entity.Id);

        return new BaseResult<Guid>(entity.Id);
    }
}
