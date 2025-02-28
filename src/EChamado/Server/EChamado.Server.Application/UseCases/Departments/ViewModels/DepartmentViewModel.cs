using EChamado.Server.Domain.Domains.Orders.ValueObjects;
using EChamado.Shared.ViewModels;

namespace EChamado.Server.Application.UseCases.Departments.ViewModels;

public class DepartmentViewModel : BaseViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DepartmentViewModel(
        Guid id,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        string name,
        string description) : base(id, createdAt, updatedAt, deletedAt)
    {
        Name = name;
        Description = description;
    }

    public static DepartmentViewModel FromEntity(Department entity)
    {
        return new DepartmentViewModel(
            entity.Id,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.DeletedAt,
            entity.Name,
            entity.Description);
    }
}
