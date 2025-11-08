using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IStatusTypeRepository _statusTypeRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IStatusTypeRepository statusTypeRepository)
    {
        _orderRepository = orderRepository;
        _statusTypeRepository = statusTypeRepository;
    }

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Busca status padrão "Aberto" ou primeiro status disponível
        var defaultStatus = await _statusTypeRepository.SearchAsync(
            x => x.Name.ToLower() == "aberto" || x.Name.ToLower() == "open",
            cancellationToken);

        var statusId = defaultStatus.FirstOrDefault()?.Id ?? Guid.NewGuid();

        var order = Order.Create(
            request.Title,
            request.Description,
            statusId,
            request.TypeId,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.RequestingUserId,
            request.RequestingUserEmail,
            request.DueDate
        );

        await _orderRepository.CreateAsync(order, cancellationToken);

        return order.Id;
    }
}
