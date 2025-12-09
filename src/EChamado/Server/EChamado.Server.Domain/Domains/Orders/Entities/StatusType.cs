using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;
using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class StatusType : SoftDeletableEntity<StatusType>
{
    
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private StatusType() : base(new StatusTypeValidation()) { }

    private StatusType(Guid id, string name, string description, IDateTimeProvider dateTimeProvider)
        : base(new StatusTypeValidation())
    {
        Id = id;
        Name = name;
        Description = description;

        MarkCreated(dateTimeProvider.UtcNow);
        Validate();
    }

    public static StatusType Create(string name, string description, IDateTimeProvider dateTimeProvider)
    {
        var statusType = new StatusType(Guid.NewGuid(), name, description, dateTimeProvider);
        statusType.AddEvent(new StatusTypeCreated(
            statusType.Id,
            statusType.Name,
            statusType.Description
        ));
        return statusType;
    }

    public void Update(string name, string description, IDateTimeProvider dateTimeProvider)
    {
        Name = name;
        Description = description;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new StatusTypeUpdated(
            Id,
            Name,
            Description
        ));
    }

    public override void Validate()
    {
        var validator = new StatusTypeValidation();
        var result = validator.Validate(this);

        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}
