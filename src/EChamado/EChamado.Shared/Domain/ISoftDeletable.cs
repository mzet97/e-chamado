namespace EChamado.Shared.Domain;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }

    void SoftDelete(DateTime utcNow);
    void Restore();
}
