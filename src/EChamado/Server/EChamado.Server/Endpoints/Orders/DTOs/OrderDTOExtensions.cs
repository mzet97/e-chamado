using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Application.UseCases.Orders.Queries;

namespace EChamado.Server.Endpoints.Orders.DTOs;

public static class OrderDTOExtensions
{
    public static CreateOrderCommand ToCommand(this CreateOrderRequest request)
    {
        return new CreateOrderCommand(
            request.Title,
            request.Description,
            request.TypeId,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.DueDate,
            request.RequestingUserId,
            request.RequestingUserEmail
        );
    }

    public static UpdateOrderCommand ToCommand(this UpdateOrderRequest request)
    {
        return new UpdateOrderCommand(
            request.Id,
            request.Title,
            request.Description,
            request.TypeId,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.DueDate
        );
    }

    public static AssignOrderCommand ToCommand(this AssignOrderRequest request)
    {
        return new AssignOrderCommand(request.OrderId, request.AssignedToUserId);
    }

    public static ChangeStatusOrderCommand ToCommand(this ChangeStatusOrderRequest request)
    {
        return new ChangeStatusOrderCommand(request.OrderId, request.StatusTypeId);
    }

    public static CloseOrderCommand ToCommand(this CloseOrderRequest request)
    {
        return new CloseOrderCommand(request.OrderId, request.Evaluation);
    }

    public static SearchOrdersQuery ToQuery(this SearchOrdersRequest request)
    {
        return new SearchOrdersQuery
        {
            PageNumber = request.PageIndex,
            PageSize = request.PageSize,
            SearchText = string.IsNullOrWhiteSpace(request.Title) && string.IsNullOrWhiteSpace(request.Description)
                ? null
                : $"{request.Title} {request.Description}".Trim(),
            StatusId = request.StatusTypeId,
            TypeId = request.TypeId,
            CategoryId = request.CategoryId,
            DepartmentId = request.DepartmentId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RequestingUserId = string.IsNullOrWhiteSpace(request.CreatedByUserId)
                ? null
                : Guid.Parse(request.CreatedByUserId),
            AssignedToUserId = string.IsNullOrWhiteSpace(request.AssignedToUserId)
                ? null
                : Guid.Parse(request.AssignedToUserId),
            IsOverdue = request.IsOverdue
        };
    }
}
