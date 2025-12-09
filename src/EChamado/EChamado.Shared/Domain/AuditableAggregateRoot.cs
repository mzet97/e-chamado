using FluentValidation;

namespace EChamado.Shared.Domain;

public abstract class AuditableAggregateRoot<T> : AuditableEntity<T>
    where T : AuditableAggregateRoot<T>
{
    protected AuditableAggregateRoot(IValidator<T> validator) : base(validator) { }
    protected AuditableAggregateRoot(IValidator<T> validator, Guid id, DateTime createdAtUtc)
        : base(validator, id, createdAtUtc) { }
}

