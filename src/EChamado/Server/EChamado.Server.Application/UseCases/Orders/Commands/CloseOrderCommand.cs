using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public record CloseOrderCommand(
    Guid OrderId,
    int Evaluation
) : IRequest<Unit>;
