using FluentValidation;

namespace EChamado.Shared.Domain;

public abstract class AuditableEntity<T> : Entity<T>, IAuditable
    where T : AuditableEntity<T>
{
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    protected AuditableEntity(IValidator<T> validator) : base(validator) { }
    protected AuditableEntity(IValidator<T> validator, Guid id, DateTime createdAtUtc)
        : base(validator, id)
    {
        CreatedAtUtc = createdAtUtc;
        Validate();
    }

    public void MarkCreated(DateTime utcNow)
    {
        if (CreatedAtUtc == default) CreatedAtUtc = utcNow;
    }

    public void MarkUpdated(DateTime utcNow) => UpdatedAtUtc = utcNow;
}
