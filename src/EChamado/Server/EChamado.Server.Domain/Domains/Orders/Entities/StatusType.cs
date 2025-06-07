using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class StatusType : Entity
{

    public string Name { get; private set; }
    public string Description { get; private set; }

    public StatusType(
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
        var validator = new StatusTypeValidation();
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

    public static StatusType Create(
        string name,
        string description)
    {
        var statusType =
            new StatusType(
                Guid.NewGuid(),
                name,
                description,
                DateTime.Now,
                null, null, false);

        statusType.Validate();

        statusType.AddEvent(
           new StatusTypeCreated(statusType));

        return statusType;
    }

    public void Update(
        string name,
        string description)
    {
        Name = name;
        Description = description;
        AddEvent(
           new StatusTypeUpdated(this));
        Validate();
    }
}
