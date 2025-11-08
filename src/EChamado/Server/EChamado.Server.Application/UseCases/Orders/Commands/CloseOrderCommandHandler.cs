using EChamado.Server.Domain.Repositories;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CloseOrderCommandHandler : IRequestHandler<CloseOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public CloseOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(CloseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");
        }

        if (order.ClosingDate.HasValue)
        {
            throw new InvalidOperationException("Order is already closed.");
        }

        order.Close(request.Evaluation);

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return Unit.Value;
    }
}
