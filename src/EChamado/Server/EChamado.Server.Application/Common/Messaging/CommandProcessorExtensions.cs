using Paramore.Brighter;

namespace EChamado.Server.Application.Common.Messaging;

/// <summary>
/// Extension methods for IAmACommandProcessor to provide MediatR-like syntax.
/// </summary>
public static class CommandProcessorExtensions
{
    /// <summary>
    /// Sends a request and returns the result.
    /// This provides a similar API to MediatR's Send method.
    /// </summary>
    public static async Task<TResult> Send<TResult>(
        this IAmACommandProcessor commandProcessor,
        BrighterRequest<TResult> request,
        CancellationToken cancellationToken = default)
    {
        // Send the request using Brighter
        await commandProcessor.SendAsync(request, cancellationToken: cancellationToken);

        // Return the result that was set by the handler
        return request.Result!;
    }

    /// <summary>
    /// Publishes an event to all handlers.
    /// This provides a similar API to MediatR's Publish method.
    /// </summary>
    public static async Task Publish<TEvent>(
        this IAmACommandProcessor commandProcessor,
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : class, IRequest
    {
        await commandProcessor.PublishAsync(@event, cancellationToken: cancellationToken);
    }
}
