using FluentValidation;

namespace EChamado.Shared.Domain;

public abstract class AggregateRoot<T> : Entity<T>
    where T : Entity<T>
{
    protected AggregateRoot(IValidator<T> validator) : base(validator) { }
    protected AggregateRoot(IValidator<T> validator, Guid id) : base(validator, id) { }
}