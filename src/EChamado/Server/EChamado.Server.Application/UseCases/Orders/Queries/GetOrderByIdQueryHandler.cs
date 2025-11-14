using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Queries;

public class GetOrderByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetOrderByIdQueryHandler> logger) :
    RequestHandlerAsync<GetOrderByIdQuery>
{
    public override async Task<GetOrderByIdQuery> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(query.OrderId);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", query.OrderId);
            throw new NotFoundException($"Order {query.OrderId} not found");
        }

        var status = await unitOfWork.StatusTypes.GetByIdAsync(order.StatusId);
        var type = await unitOfWork.OrderTypes.GetByIdAsync(order.TypeId);
        var category = await unitOfWork.Categories.GetByIdAsync(order.CategoryId);
        var department = await unitOfWork.Departments.GetByIdAsync(order.DepartmentId);

        var subCategory = order.SubCategoryId.HasValue
            ? await unitOfWork.SubCategories.GetByIdAsync(order.SubCategoryId.Value)
            : null;

        // Converter Evaluation de string para int?
        int? evaluation = null;
        if (!string.IsNullOrEmpty(order.Evaluation) && int.TryParse(order.Evaluation, out var evalValue))
        {
            evaluation = evalValue;
        }

        var viewModel = new OrderViewModel(
            order.Id,
            order.Title,
            order.Description,
            evaluation,
            order.OpeningDate ?? DateTime.UtcNow,
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

        logger.LogInformation("Order {OrderId} retrieved successfully", query.OrderId);

        query.Result = new BaseResult<OrderViewModel>(viewModel);

        return await base.HandleAsync(query, cancellationToken);
    }
}
