namespace EChamado.Shared.Domain;

public interface IEntity
{
    Guid Id { get; }
    IReadOnlyCollection<IDomainEvent> Events { get; }
    void ClearEvents();
}