using EChamado.Shared.Shared;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class SubCategoryValidation : AbstractValidator<SubCategory>
{
    public SubCategoryValidation()
    {
        Include(new EntityValidation());

        RuleFor(subCategory => subCategory.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(subCategory => subCategory.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(subCategory => subCategory.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");
    }
}
