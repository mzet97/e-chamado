using Paramore.Brighter;

namespace EChamado.Server.Application.Common.Messaging;

/// <summary>
/// Extension methods for IAmACommandProcessor adding helpers that return the Result.
/// </summary>
public static class CommandProcessorExtensions
{
    /// <summary>
    /// Sends a request and returns the Result populated by its handler.
    /// </summary>
    public static async Task<TResult> SendWithResultAsync<TResult>(
        this IAmACommandProcessor commandProcessor,
        BrighterRequest<TResult> request,
        CancellationToken cancellationToken = default)
    {
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);
        return request.Result!;
    }

    /// <summary>
    /// Publishes an event (alias that keeps naming consistent).
    /// </summary>
    public static Task PublishWithResultAsync<TEvent>(
        this IAmACommandProcessor commandProcessor,
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : class, IRequest
        => commandProcessor.PublishAsync(@event, cancellationToken: cancellationToken);
}
