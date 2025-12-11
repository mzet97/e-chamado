using FluentValidation;

namespace EChamado.Server.Domain.Domains.Orders.Entities.Validations;

public class CommentValidation : AbstractValidator<Comment>
{
    public CommentValidation()
    {
        RuleFor(x => x.Text)
            .Must(text => !string.IsNullOrWhiteSpace(text))
            .WithMessage("O texto do comentário é obrigatório")
            .MaximumLength(2000)
            .WithMessage("O texto do comentário deve ter no máximo 2000 caracteres");

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("O ID do chamado é obrigatório");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório");

        RuleFor(x => x.UserEmail)
            .NotEmpty()
            .WithMessage("O e-mail do usuário é obrigatório")
            .EmailAddress()
            .WithMessage("O e-mail do usuário deve ser válido");
    }
}
