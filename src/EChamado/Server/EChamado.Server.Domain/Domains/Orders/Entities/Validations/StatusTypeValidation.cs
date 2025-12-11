using EChamado.Shared.Domain;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class StatusTypeValidation : AbstractValidator<StatusType>
{
    public StatusTypeValidation()
    {
        Include(new EntityValidation<StatusType>());

        RuleFor(statusType => statusType.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(statusType => statusType.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}