using Microsoft.Extensions.DependencyInjection;
using Paramore.Darker;

namespace EChamado.Server.Application.Common.Messaging;

public sealed class ServiceProviderQueryProcessor : IQueryProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProviderQueryProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Executes a query synchronously. This method is obsolete and should be avoided.
    /// </summary>
    /// <remarks>
    /// This method calls ExecuteAsync(...).GetAwaiter().GetResult(), which can lead to deadlocks 
    /// in certain synchronization contexts (e.g., ASP.NET Core pre-.NET 6 scenarios or when using 
    /// ConfigureAwait(false) incorrectly). Use ExecuteAsync instead.
    /// </remarks>
    [Obsolete("This synchronous method can cause deadlocks. Use ExecuteAsync instead.")]
    public TResult Execute<TResult>(IQuery<TResult> query)
    {
        return ExecuteAsync(query).GetAwaiter().GetResult();
    }

    public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        dynamic? handler = _serviceProvider.GetService(handlerType);

        if (handler is null)
        {
            throw new InvalidOperationException($"No handler registered for query type {query.GetType().Name}");
        }

        return await handler.ExecuteAsync((dynamic)query, cancellationToken);
    }
}
