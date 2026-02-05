using FluentValidation;

namespace EChamado.Shared.Domain;

public abstract class SoftDeletableEntity<T> : AuditableEntity<T>, ISoftDeletable
    where T : SoftDeletableEntity<T>
{
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    protected SoftDeletableEntity(IValidator<T> validator) : base(validator) { }
    protected SoftDeletableEntity(IValidator<T> validator, Guid id, DateTime createdAtUtc)
        : base(validator, id, createdAtUtc) { }

    public void SoftDelete(DateTime utcNow)
    {
        IsDeleted = true;
        DeletedAtUtc = utcNow;
        Validate();
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        Validate();
    }
}