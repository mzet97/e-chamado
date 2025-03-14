using EChamado.Shared.Shared;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class CategoryValidation : AbstractValidator<Category>
{
    public CategoryValidation()
    {
        Include(new EntityValidation());

        RuleFor(category => category.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(category => category.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
