using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class CreateOrderCommand : BrighterRequest<BaseResult<Guid>>
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TypeId { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid RequestingUserId { get; set; } = default!;
    public string RequestingUserEmail { get; set; } = string.Empty;

    public CreateOrderCommand()
    {
    }

    public CreateOrderCommand(string title, string description, Guid typeId, Guid? categoryId, Guid? subCategoryId, Guid? departmentId, DateTime? dueDate, Guid requestingUserId, string requestingUserEmail)
    {
        Title = title;
        Description = description;
        TypeId = typeId;
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
        DepartmentId = departmentId;
        DueDate = dueDate;
        RequestingUserId = requestingUserId;
        RequestingUserEmail = requestingUserEmail;
    }
}
