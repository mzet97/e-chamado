namespace EChamado.Server.Application.UseCases.Orders.ViewModels;

public record OrderViewModel(
    Guid Id,
    string Title,
    string Description,
    int? Evaluation,
    DateTime OpeningDate,
    DateTime? ClosingDate,
    DateTime? DueDate,
    Guid StatusId,
    string StatusName,
    Guid TypeId,
    string TypeName,
    Guid? CategoryId,
    string? CategoryName,
    Guid? SubCategoryId,
    string? SubCategoryName,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid RequestingUserId,
    string RequestingUserEmail,
    Guid? ResponsibleUserId,
    string? ResponsibleUserEmail,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeletedAtUtc,
    bool IsDeleted
);
