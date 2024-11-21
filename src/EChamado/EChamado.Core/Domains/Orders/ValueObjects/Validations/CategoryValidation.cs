using FluentValidation;

namespace EChamado.Core.Domains.Orders.ValueObjects.Validations;

public class CategoryValidation : AbstractValidator<Category>
{
    public CategoryValidation()
    {
        RuleFor(category => category.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(category => category.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
