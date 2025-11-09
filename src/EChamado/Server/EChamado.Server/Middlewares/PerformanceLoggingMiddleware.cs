using System.Diagnostics;

namespace EChamado.Server.Middlewares;

/// <summary>
/// Middleware para medir e logar performance de requisições lentas
/// </summary>
public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceLoggingMiddleware> _logger;
    private readonly int _slowRequestThresholdMs;

    public PerformanceLoggingMiddleware(
        RequestDelegate next,
        ILogger<PerformanceLoggingMiddleware> logger,
        int slowRequestThresholdMs = 3000)
    {
        _next = next;
        _logger = logger;
        _slowRequestThresholdMs = slowRequestThresholdMs;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Loga apenas requisições lentas
            if (stopwatch.ElapsedMilliseconds > _slowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "SLOW REQUEST detected - {Method} {Path} took {Duration}ms (threshold: {Threshold}ms) - StatusCode: {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    _slowRequestThresholdMs,
                    context.Response.StatusCode);
            }
            else
            {
                _logger.LogDebug(
                    "Request completed - {Method} {Path} took {Duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
