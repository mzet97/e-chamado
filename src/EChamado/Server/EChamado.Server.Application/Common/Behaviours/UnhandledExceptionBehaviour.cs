using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.Common.Behaviours;

/// <summary>
/// Attribute to mark handlers that should have exception logging.
/// </summary>
public class RequestLoggingAttribute : RequestHandlerAttribute
{
    public RequestLoggingAttribute(int step, HandlerTiming timing = HandlerTiming.Before)
        : base(step, timing)
    {
    }

    public override Type GetHandlerType()
    {
        return typeof(UnhandledExceptionHandler<>);
    }
}

/// <summary>
/// Exception handling and logging handler for Brighter pipeline.
/// Logs unhandled exceptions before re-throwing them.
/// </summary>
public class UnhandledExceptionHandler<TRequest> : RequestHandlerAsync<TRequest>
    where TRequest : class, IRequest
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionHandler(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public override async Task<TRequest> HandleAsync(TRequest command, CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.HandleAsync(command, cancellationToken);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "Request: Unhandled Exception for Request {Name} {@Request}", requestName, command);

            throw;
        }
    }
}