using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class AssignOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<AssignOrderCommandHandler> logger) :
    IRequestHandler<AssignOrderCommand, BaseResult>
{
    public async Task<BaseResult> Handle(AssignOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", request.OrderId);
            throw new NotFoundException($"Order {request.OrderId} not found");
        }

        // Busca usuário para obter email
        var user = await unitOfWork.Users.GetByIdAsync(request.AssignedToUserId, cancellationToken);

        if (user == null)
        {
            logger.LogError("User {UserId} not found", request.AssignedToUserId);
            throw new NotFoundException($"User {request.AssignedToUserId} not found");
        }

        order.AssignTo(request.AssignedToUserId, user.Email ?? string.Empty);

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} assigned to user {UserId}", request.OrderId, request.AssignedToUserId);

        return new BaseResult();
    }
}
