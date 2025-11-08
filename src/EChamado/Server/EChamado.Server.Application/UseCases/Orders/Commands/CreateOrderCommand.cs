using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public record CreateOrderCommand(
    string Title,
    string Description,
    Guid TypeId,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate,
    Guid RequestingUserId,
    string RequestingUserEmail
) : IRequest<Guid>;
