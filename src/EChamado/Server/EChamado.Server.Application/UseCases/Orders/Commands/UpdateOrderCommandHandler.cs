using EChamado.Server.Domain.Repositories;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {request.Id} not found.");
        }

        order.Update(
            request.Title,
            request.Description,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.DueDate
        );

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return Unit.Value;
    }
}
