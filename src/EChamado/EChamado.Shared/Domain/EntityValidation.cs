using FluentValidation;

namespace EChamado.Shared.Domain;

public class EntityValidation<T> : AbstractValidator<T>
    where T : IEntity
{
    public EntityValidation()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id inválido.");
    }
}