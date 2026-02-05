using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;
using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class OrderType : SoftDeletableEntity<OrderType>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private OrderType() : base(new OrderTypeValidation()) { }

    private OrderType(Guid id, string name, string description, IDateTimeProvider dateTimeProvider)
        : base(new OrderTypeValidation())
    {
        Id = id;
        Name = name;
        Description = description;

        MarkCreated(dateTimeProvider.UtcNow);
        Validate();
    }

    public static OrderType Create(string name, string description, IDateTimeProvider dateTimeProvider)
    {
        var orderType = new OrderType(Guid.NewGuid(), name, description, dateTimeProvider);
        orderType.AddEvent(new OrderTypeCreated(
            orderType.Id,
            orderType.Name,
            orderType.Description
        ));
        return orderType;
    }

    public void Update(string name, string description, IDateTimeProvider dateTimeProvider)
    {
        Name = name;
        Description = description;

        MarkUpdated(dateTimeProvider.UtcNow);
        Validate();

        AddEvent(new OrderTypeUpdated(
            Id,
            Name,
            Description
        ));
    }

    public override void Validate()
    {
        var validator = new OrderTypeValidation();
        var result = validator.Validate(this);

        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}
