using EChamado.Shared.Domain;

namespace EChamado.Server.Infrastructure.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}
