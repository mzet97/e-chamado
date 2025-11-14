using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.Departments;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class Department : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Department(
        Guid id,
        string name,
        string description,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        bool isDeleted) : base(id, createdAt, updatedAt, deletedAt, isDeleted)
    {
        Name = name;
        Description = description;
        Validate();
    }

    public override void Validate()
    {
        var validator = new DepartmentValidation();
        var result = validator.Validate(this);
        if (!result.IsValid)
        {
            _errors = result.Errors.Select(x => x.ErrorMessage);
            _isValid = false;
        }
        else
        {
            _errors = Enumerable.Empty<string>();
            _isValid = true;
        }
    }

    public static Department Create(
        string name,
        string description)
    {
        var department =
            new Department(
                Guid.NewGuid(),
                name,
                description,
                DateTime.Now,
                null, null, false);

        department.AddEvent(
            new DepartmentCreated(department));
        return department;
    }

    public void Update(
        string name,
        string description)
    {
        Name = name;
        Description = description;

        Update(); // Chama base.Update() que define UpdatedAt

        AddEvent(
            new DepartmentUpdated(this));
    }
}
