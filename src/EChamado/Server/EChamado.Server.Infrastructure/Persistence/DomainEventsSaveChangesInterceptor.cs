using EChamado.Server.Infrastructure.Events;
using EChamado.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EChamado.Server.Infrastructure.Persistence;

public class DomainEventsSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventsSaveChangesInterceptor(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var domainEntities = eventData.Context.ChangeTracker
            .Entries<IEntity>()
            .Where(entry => entry.Entity.Events.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(entry => entry.Entity.Events)
            .ToList();

        if (domainEvents.Count != 0)
        {
            await _dispatcher.DispatchAsync(domainEvents, cancellationToken);
        }

        foreach (var entry in domainEntities)
        {
            entry.Entity.ClearEvents();
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
