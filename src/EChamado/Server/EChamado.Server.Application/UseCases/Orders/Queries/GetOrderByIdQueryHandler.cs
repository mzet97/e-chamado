using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class GetOrderByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetOrderByIdQueryHandler> logger) :
    IRequestHandler<GetOrderByIdQuery, BaseResult<OrderViewModel>>
{
    public async Task<BaseResult<OrderViewModel>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", request.OrderId);
            throw new NotFoundException($"Order {request.OrderId} not found");
        }

        var status = await unitOfWork.StatusTypes.GetByIdAsync(order.StatusId, cancellationToken);
        var type = await unitOfWork.OrderTypes.GetByIdAsync(order.TypeId, cancellationToken);

        var category = order.CategoryId.HasValue
            ? await unitOfWork.Categories.GetByIdAsync(order.CategoryId.Value, cancellationToken)
            : null;

        var subCategory = order.SubCategoryId.HasValue
            ? await unitOfWork.SubCategories.GetByIdAsync(order.SubCategoryId.Value, cancellationToken)
            : null;

        var department = order.DepartmentId.HasValue
            ? await unitOfWork.Departments.GetByIdAsync(order.DepartmentId.Value, cancellationToken)
            : null;

        var viewModel = new OrderViewModel(
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

        logger.LogInformation("Order {OrderId} retrieved successfully", request.OrderId);

        return new BaseResult<OrderViewModel>(viewModel);
    }
}
