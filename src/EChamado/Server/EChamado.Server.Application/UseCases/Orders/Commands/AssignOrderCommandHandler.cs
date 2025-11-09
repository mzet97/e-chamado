using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class AssignOrderCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<AssignOrderCommandHandler> logger) :
    RequestHandlerAsync<AssignOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<AssignOrderCommand> HandleAsync(AssignOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", command.OrderId);
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        // Busca usuário para obter email
        var user = await unitOfWork.Users.GetByIdAsync(command.AssignedToUserId, cancellationToken);

        if (user == null)
        {
            logger.LogError("User {UserId} not found", command.AssignedToUserId);
            throw new NotFoundException($"User {command.AssignedToUserId} not found");
        }

        order.AssignTo(command.AssignedToUserId, user.Email ?? string.Empty);

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order, cancellationToken);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} assigned to user {UserId}", command.OrderId, command.AssignedToUserId);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
