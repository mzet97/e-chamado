using EChamado.Server.Application.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Repositories;
using Paramore.Darker;

namespace EChamado.Server.Application.Orders.QueryHandlers;

public sealed class GetOrderByIdQueryHandler : QueryHandlerAsync<GetOrderByIdQuery, OrderViewModel?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<OrderViewModel?> ExecuteAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(query.OrderId);
        if (order is null)
        {
            return null;
        }

        var status = await _unitOfWork.StatusTypes.GetByIdAsync(order.StatusId);
        var type = await _unitOfWork.OrderTypes.GetByIdAsync(order.TypeId);
        var category = await _unitOfWork.Categories.GetByIdAsync(order.CategoryId);
        var department = await _unitOfWork.Departments.GetByIdAsync(order.DepartmentId);
        var subCategory = order.SubCategoryId.HasValue
            ? await _unitOfWork.SubCategories.GetByIdAsync(order.SubCategoryId.Value)
            : null;

        int? evaluation = null;
        if (!string.IsNullOrEmpty(order.Evaluation) && int.TryParse(order.Evaluation, out var parsed))
        {
            evaluation = parsed;
        }

        return new OrderViewModel(
            order.Id,
            order.Title,
            order.Description,
            evaluation,
            order.OpeningDate ?? DateTime.UtcNow,
            order.ClosingDate,
            order.DueDate,
            order.StatusId,
            status?.Name ?? string.Empty,
            order.TypeId,
            type?.Name ?? string.Empty,
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
            order.CreatedAtUtc,
            order.UpdatedAtUtc ?? order.CreatedAtUtc,
            order.DeletedAtUtc,
            order.IsDeleted);
    }
}
