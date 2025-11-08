using EChamado.Server.Domain.Repositories;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class AssignOrderCommandHandler : IRequestHandler<AssignOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public AssignOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(AssignOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found.");
        }

        // Atualiza o responsável
        order.ResponsibleUserId = request.ResponsibleUserId;
        order.ResponsibleUserEmail = request.ResponsibleUserEmail;
        order.UpdatedAt = DateTime.UtcNow;

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return Unit.Value;
    }
}
