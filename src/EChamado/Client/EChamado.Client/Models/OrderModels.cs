namespace EChamado.Client.Models;

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
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<CommentResponse>? Comments = null
)
{
    public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.Now && !ClosingDate.HasValue;
};

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

public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
)
{
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public record CreateOrderRequest(
    string Title,
    string Description,
    Guid TypeId,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);

public record UpdateOrderRequest(
    string Title,
    string Description,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);

public record CloseOrderRequest(int Evaluation);
