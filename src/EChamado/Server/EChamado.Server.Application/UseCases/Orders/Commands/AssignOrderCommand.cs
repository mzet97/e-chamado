using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public record AssignOrderCommand(
    Guid OrderId,
    Guid ResponsibleUserId,
    string ResponsibleUserEmail
) : IRequest<Unit>;
