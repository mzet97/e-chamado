using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.ViewModels;

namespace EChamado.Server.Application.UseCases.Departments.ViewModels;

public class DepartmentViewModel : BaseViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DepartmentViewModel(
        Guid id,
        DateTime createdAtUtc,
        DateTime? updatedAtUtc,
        DateTime? deletedAtUtc,
        bool isDeleted,
        string name,
        string description) : base(id, createdAtUtc, updatedAtUtc, deletedAtUtc, isDeleted)
    {
        Name = name;
        Description = description;
    }

    public static DepartmentViewModel FromEntity(Department entity)
    {
        return new DepartmentViewModel(
            entity.Id,
            entity.CreatedAtUtc,
            entity.UpdatedAtUtc,
            entity.DeletedAtUtc,
            entity.IsDeleted,
            entity.Name,
            entity.Description);
    }
}
