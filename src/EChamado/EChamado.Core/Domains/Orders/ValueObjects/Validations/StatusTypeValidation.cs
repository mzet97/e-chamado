using FluentValidation;

namespace EChamado.Core.Domains.Orders.ValueObjects.Validations;

public class StatusTypeValidation : AbstractValidator<StatusType>
{
    public StatusTypeValidation()
    {
        RuleFor(statusType => statusType.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(statusType => statusType.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}