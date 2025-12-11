namespace EChamado.Shared.Domain;

public interface IAuditable
{
    DateTime CreatedAtUtc { get; }
    DateTime? UpdatedAtUtc { get; }

    void MarkCreated(DateTime utcNow);
    void MarkUpdated(DateTime utcNow);
}