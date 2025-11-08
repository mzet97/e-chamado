namespace EChamado.Server.Application.UseCases.Orders.ViewModels;

public record OrderListViewModel(
    Guid Id,
    string Title,
    DateTime OpeningDate,
    DateTime? ClosingDate,
    DateTime? DueDate,
    string StatusName,
    string TypeName,
    string? DepartmentName,
    string RequestingUserEmail,
    string? ResponsibleUserEmail,
    bool IsOverdue
);
