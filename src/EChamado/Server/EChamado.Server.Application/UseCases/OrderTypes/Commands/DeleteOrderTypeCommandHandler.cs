using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.OrderTypes.Commands;

public class DeleteOrderTypeCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteOrderTypeCommandHandler> logger) :
    IRequestHandler<DeleteOrderTypeCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteOrderTypeCommand request, CancellationToken cancellationToken)
    {
        var orderType = await unitOfWork.OrderTypes.GetByIdAsync(request.OrderTypeId, cancellationToken);

        if (orderType == null)
        {
            logger.LogError("OrderType {OrderTypeId} not found", request.OrderTypeId);
            throw new NotFoundException($"OrderType {request.OrderTypeId} not found");
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.OrderTypes.DeleteAsync(orderType, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("OrderType {OrderTypeId} deleted successfully", request.OrderTypeId);

        return new BaseResult();
    }
}
