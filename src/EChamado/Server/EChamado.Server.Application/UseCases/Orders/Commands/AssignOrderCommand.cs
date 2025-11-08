using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public record AssignOrderCommand(
    Guid OrderId,
    Guid AssignedToUserId
) : IRequest<BaseResult>;
