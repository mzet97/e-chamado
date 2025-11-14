using EChamado.Shared.Shared;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class CategoryValidation : AbstractValidator<Category>
{
    public CategoryValidation()
    {
        Include(new EntityValidation());

        RuleFor(category => category.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("O nome da categoria é obrigatório.")
            .MinimumLength(2).WithMessage("O nome da categoria deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome da categoria deve ter no máximo 100 caracteres.");

        RuleFor(category => category.Description)
            .Must(description => !string.IsNullOrWhiteSpace(description))
                .WithMessage("A descrição da categoria é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição da categoria deve ter no máximo 500 caracteres.");
    }
}
