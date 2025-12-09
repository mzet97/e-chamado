using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.Departments;
using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class Department : SoftDeletableEntity<Department>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Department() : base(new DepartmentValidation()) { }

    private Department(Guid id, string name, string description, IDateTimeProvider dateTimeProvider)
        : base(new DepartmentValidation())
    {
        Id = id;
        Name = name;
        Description = description;

        MarkCreated(dateTimeProvider.UtcNow);
        Validate();
    }

    public static Department Create(string name, string description, IDateTimeProvider dateTimeProvider)
    {
        var department = new Department(Guid.NewGuid(), name, description, dateTimeProvider);
        department.AddEvent(new DepartmentCreated(
            department.Id,
            department.Name,
            department.Description
        ));
        return department;
    }

    public void Update(string name, string description, IDateTimeProvider dateTimeProvider)
    {
        Name = name;
        Description = description;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new DepartmentUpdated(
            Id,
            Name,
            Description
        ));
    }

    public override void Validate()
    {
        var validator = new DepartmentValidation();
        var result = validator.Validate(this);

        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}
