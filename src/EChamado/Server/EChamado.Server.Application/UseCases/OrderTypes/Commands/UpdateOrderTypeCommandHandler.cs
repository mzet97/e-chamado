using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class UpdateOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateOrderTypeCommandHandler> logger) :
    IRequestHandler<UpdateOrderTypeCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateOrderTypeCommand request, CancellationToken cancellationToken)
    {
        var orderType = await unitOfWork.OrderTypes.GetByIdAsync(request.Id, cancellationToken);

        if (orderType == null)
        {
            logger.LogError("OrderType {OrderTypeId} not found", request.Id);
            throw new NotFoundException($"OrderType {request.Id} not found");
        }

        orderType.Update(request.Name, request.Description);

        if (!orderType.IsValid())
        {
            logger.LogError("Validate OrderType has error");
            throw new ValidationException("Validate OrderType has error", orderType.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.OrderTypes.UpdateAsync(orderType, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("OrderType {OrderTypeId} updated successfully", request.Id);

        return new BaseResult();
    }
}
