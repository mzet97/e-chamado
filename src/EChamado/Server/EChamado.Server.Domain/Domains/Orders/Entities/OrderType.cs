using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class OrderType : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public OrderType(
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
        var validator = new OrderTypeValidation();
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

    public static OrderType Create(
        string name,
        string description)
    {
        var orderType =
            new OrderType(
                Guid.NewGuid(),
                name,
                description,
                DateTime.Now,
                null, null, false);

        orderType.AddEvent(
            new OrderTypeCreated(orderType));
        return orderType;
    }

    public void Update(
        string name,
        string description)
    {
        Name = name;
        Description = description;
        Validate();
        AddEvent(
            new OrderTypeUpdated(this));
    }
}
