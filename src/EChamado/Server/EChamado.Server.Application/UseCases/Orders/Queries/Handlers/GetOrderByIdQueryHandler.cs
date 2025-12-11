using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Orders.Queries.Handlers;

public class GetOrderByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetOrderByIdQueryHandler> logger) :
    RequestHandlerAsync<GetOrderByIdQuery>
{
    public override async Task<GetOrderByIdQuery> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdWithIncludesAsync(query.OrderId);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", query.OrderId);
            throw new NotFoundException($"Order {query.OrderId} not found");
        }

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
            order.Status?.Name ?? "Unknown",
            order.TypeId,
            order.Type?.Name ?? "Unknown",
            order.CategoryId,
            order.Category?.Name,
            order.SubCategoryId,
            order.SubCategory?.Name,
            order.DepartmentId,
            order.Department?.Name,
            order.RequestingUserId,
            order.RequestingUserEmail,
            order.ResponsibleUserId,
            order.ResponsibleUserEmail,
            order.CreatedAtUtc,
            order.UpdatedAtUtc,
            order.DeletedAtUtc,
            order.IsDeleted
        );

        logger.LogInformation("Order {OrderId} retrieved successfully", query.OrderId);

        query.Result = new BaseResult<OrderViewModel>(viewModel);

        return await base.HandleAsync(query, cancellationToken);
    }
}
