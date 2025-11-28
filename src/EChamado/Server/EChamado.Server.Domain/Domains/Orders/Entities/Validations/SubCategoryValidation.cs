using EChamado.Shared.Shared;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class SubCategoryValidation : AbstractValidator<SubCategory>
{
    public SubCategoryValidation()
    {
        Include(new EntityValidation());

        RuleFor(subCategory => subCategory.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("O nome da subcategoria é obrigatório.")
            .MinimumLength(2).WithMessage("O nome da subcategoria deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome da subcategoria deve ter no máximo 100 caracteres.");

        RuleFor(subCategory => subCategory.Description)
            .Must(description => !string.IsNullOrWhiteSpace(description))
                .WithMessage("A descrição da subcategoria é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição da subcategoria deve ter no máximo 500 caracteres.");

        RuleFor(subCategory => subCategory.CategoryId)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");
    }
}
