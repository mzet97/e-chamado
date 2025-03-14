using EChamado.Shared.Shared;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class OrderTypeValidation : AbstractValidator<OrderType>
{
    public OrderTypeValidation()
    {
        Include(new EntityValidation());

        RuleFor(orderType => orderType.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(orderType => orderType.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
