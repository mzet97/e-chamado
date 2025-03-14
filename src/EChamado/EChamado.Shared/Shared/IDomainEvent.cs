namespace EChamado.Shared.Shared;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
