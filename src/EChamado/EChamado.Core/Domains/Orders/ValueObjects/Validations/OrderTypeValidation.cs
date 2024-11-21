using FluentValidation;

namespace EChamado.Core.Domains.Orders.ValueObjects.Validations;

public class OrderTypeValidation : AbstractValidator<OrderType>
{
    public OrderTypeValidation()
    {
        RuleFor(orderType => orderType.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(orderType => orderType.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
