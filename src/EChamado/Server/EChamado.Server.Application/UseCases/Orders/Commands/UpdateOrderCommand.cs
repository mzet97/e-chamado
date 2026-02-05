using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Orders.Commands;

public class UpdateOrderCommand : BrighterRequest<BaseResult>
{
    public Guid Id { get; set; } = default!;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid TypeId { get; set; } = default!;
    public Guid? CategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? DueDate { get; set; }

    public UpdateOrderCommand()
    {
    }

    public UpdateOrderCommand(Guid id, string title, string description, Guid typeId, Guid? categoryId, Guid? subCategoryId, Guid? departmentId, DateTime? dueDate)
    {
        Id = id;
        Title = title;
        Description = description;
        TypeId = typeId;
        CategoryId = categoryId;
        SubCategoryId = subCategoryId;
        DepartmentId = departmentId;
        DueDate = dueDate;
    }
}
