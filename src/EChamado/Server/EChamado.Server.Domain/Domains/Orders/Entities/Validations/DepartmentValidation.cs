using EChamado.Shared.Domain;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class DepartmentValidation : AbstractValidator<Department>
{
    public DepartmentValidation()
    {
        Include(new EntityValidation<Department>());

        RuleFor(department => department.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("O nome do departamento é obrigatório.")
            .MinimumLength(2).WithMessage("O nome do departamento deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome do departamento deve ter no máximo 100 caracteres.");

        RuleFor(department => department.Description)
            .Must(description => !string.IsNullOrWhiteSpace(description))
                .WithMessage("A descrição do departamento é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição do departamento deve ter no máximo 500 caracteres.");
    }
}
