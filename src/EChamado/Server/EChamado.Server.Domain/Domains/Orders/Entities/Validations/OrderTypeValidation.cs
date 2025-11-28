using EChamado.Shared.Shared;
using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class OrderTypeValidation : AbstractValidator<OrderType>
{
    public OrderTypeValidation()
    {
        Include(new EntityValidation());

        RuleFor(orderType => orderType.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("O nome do tipo de chamado é obrigatório.")
            .MinimumLength(2).WithMessage("O nome do tipo de chamado deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome do tipo de chamado deve ter no máximo 100 caracteres.");

        RuleFor(orderType => orderType.Description)
            .Must(description => !string.IsNullOrWhiteSpace(description))
                .WithMessage("A descrição do tipo de chamado é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição do tipo de chamado deve ter no máximo 500 caracteres.");
    }
}
