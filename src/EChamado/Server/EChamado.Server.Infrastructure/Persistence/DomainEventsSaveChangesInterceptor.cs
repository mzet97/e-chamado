using EChamado.Server.Infrastructure.Events;
using EChamado.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EChamado.Server.Infrastructure.Persistence;

/// <summary>
/// Interceptor that automatically dispatches domain events after SaveChanges completes successfully.
/// </summary>
/// <remarks>
/// <para>
/// This interceptor hooks into the Entity Framework Core SaveChanges lifecycle to automatically
/// dispatch domain events collected on entities after the database transaction has completed.
/// </para>
/// <para>
/// <strong>Timing:</strong> Events are dispatched in the SavedChangesAsync override, which executes
/// AFTER the database changes have been committed. This ensures that event handlers operate on
/// persisted data and can safely perform additional database operations if needed.
/// </para>
/// <para>
/// <strong>Transaction Consistency:</strong> Since events are dispatched after the save operation
/// completes, the domain events are dispatched outside the original transaction boundary. This means:
/// <list type="bullet">
/// <item>Event handlers see the committed state of the database</item>
/// <item>Failures in event handlers will NOT roll back the original save operation</item>
/// <item>Event handlers should implement their own error handling and compensating logic</item>
/// <item>For critical consistency requirements, consider using the outbox pattern or two-phase commit</item>
/// </list>
/// </para>
/// </remarks>
public class DomainEventsSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    public DomainEventsSaveChangesInterceptor(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Dispatches domain events after the save operation has completed successfully.
    /// </summary>
    /// <param name="eventData">Context information about the save operation.</param>
    /// <param name="result">The result returned by the base save operation.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>The number of entities saved to the database.</returns>
    /// <remarks>
    /// This method collects all domain events from entities that implement IEntity,
    /// dispatches them via the IDomainEventDispatcher, and then clears the events
    /// from the entities to prevent duplicate dispatching.
    /// </remarks>
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
