using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Repositories;
using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderViewModel?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IStatusTypeRepository _statusTypeRepository;
    private readonly IOrderTypeRepository _orderTypeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public GetOrderByIdQueryHandler(
        IOrderRepository orderRepository,
        IStatusTypeRepository statusTypeRepository,
        IOrderTypeRepository orderTypeRepository,
        ICategoryRepository categoryRepository,
        ISubCategoryRepository subCategoryRepository,
        IDepartmentRepository departmentRepository)
    {
        _orderRepository = orderRepository;
        _statusTypeRepository = statusTypeRepository;
        _orderTypeRepository = orderTypeRepository;
        _categoryRepository = categoryRepository;
        _subCategoryRepository = subCategoryRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<OrderViewModel?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
            return null;

        var status = await _statusTypeRepository.GetByIdAsync(order.StatusId, cancellationToken);
        var type = await _orderTypeRepository.GetByIdAsync(order.TypeId, cancellationToken);
        var category = order.CategoryId.HasValue
            ? await _categoryRepository.GetByIdAsync(order.CategoryId.Value, cancellationToken)
            : null;
        var subCategory = order.SubCategoryId.HasValue
            ? await _subCategoryRepository.GetByIdAsync(order.SubCategoryId.Value, cancellationToken)
            : null;
        var department = order.DepartmentId.HasValue
            ? await _departmentRepository.GetByIdAsync(order.DepartmentId.Value, cancellationToken)
            : null;

        return new OrderViewModel(
            order.Id,
            order.Title,
            order.Description,
            order.Evaluation,
            order.OpeningDate,
            order.ClosingDate,
            order.DueDate,
            order.StatusId,
            status?.Name ?? "Unknown",
            order.TypeId,
            type?.Name ?? "Unknown",
            order.CategoryId,
            category?.Name,
            order.SubCategoryId,
            subCategory?.Name,
            order.DepartmentId,
            department?.Name,
            order.RequestingUserId,
            order.RequestingUserEmail,
            order.ResponsibleUserId,
            order.ResponsibleUserEmail,
            order.CreatedAt,
            order.UpdatedAt
        );
    }
}
