using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Orders.Commands.Handlers;

public class AssignOrderCommandHandler(
    IUnitOfWork unitOfWork,
    IUserReadRepository userReadRepository,
    IDateTimeProvider dateTimeProvider,
    ILogger<AssignOrderCommandHandler> logger) :
    RequestHandlerAsync<AssignOrderCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<AssignOrderCommand> HandleAsync(AssignOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", command.OrderId);
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        // Resolve o email do usuário responsável a partir do ID informado.
        var responsible = await userReadRepository.GetByIdAsync(command.AssignedToUserId, cancellationToken);
        if (responsible == null)
        {
            logger.LogError("Responsible user {UserId} not found", command.AssignedToUserId);
            throw new NotFoundException($"Responsible user {command.AssignedToUserId} not found");
        }

        order.AssignTo(command.AssignedToUserId, responsible.Email, dateTimeProvider);

        if (!order.IsValid())
        {
            logger.LogError("Validate Order has error");
            throw new ValidationException("Validate Order has error", order.Errors);
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Orders.UpdateAsync(order);

        await unitOfWork.CommitAsync();

        logger.LogInformation("Order {OrderId} assigned to user {UserId} ({Email})",
            command.OrderId, command.AssignedToUserId, responsible.Email);

        command.Result = new BaseResult();
        return await base.HandleAsync(command, cancellationToken);
    }
}
