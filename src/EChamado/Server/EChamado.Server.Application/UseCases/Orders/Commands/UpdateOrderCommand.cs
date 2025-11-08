using MediatR;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public record UpdateOrderCommand(
    Guid Id,
    string Title,
    string Description,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
) : IRequest<Unit>;
