using FluentValidation;

namespace EChamado.Core.Domains.Orders.ValueObjects.Validations;

public class SubCategoryValidation : AbstractValidator<SubCategory>
{
    public SubCategoryValidation()
    {
        RuleFor(subCategory => subCategory.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(subCategory => subCategory.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(subCategory => subCategory.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");
    }
}
