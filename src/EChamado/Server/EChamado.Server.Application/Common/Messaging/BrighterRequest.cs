using Paramore.Brighter;

namespace EChamado.Server.Application.Common.Messaging;

/// <summary>
/// Base class for requests that return a result.
/// This allows Brighter to work with return values similar to MediatR.
/// </summary>
public abstract class BrighterRequest<TResult> : IRequest
{
    /// <summary>
    /// The result of the request, set by the handler.
    /// </summary>
    public TResult? Result { get; set; }

    /// <summary>
    /// Unique identifier for this request.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
}
