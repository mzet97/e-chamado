using EChamado.Server.Domain.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class CreateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
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

        logger.LogInformation("OrderType {OrderTypeId} created successfully", entity.Id);

        return new BaseResult<Guid>(entity.Id);
    }
}
