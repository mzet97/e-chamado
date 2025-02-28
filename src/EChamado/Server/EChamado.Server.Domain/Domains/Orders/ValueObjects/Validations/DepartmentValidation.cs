using EChamado.Server.Domain.Domains.Orders.ValueObjects;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.ValueObjects.Validations;

public class DepartmentValidation : AbstractValidator<Department>
{
    public DepartmentValidation()
    {
        RuleFor(department => department.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(department => department.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
