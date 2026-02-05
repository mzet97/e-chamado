using System.Diagnostics;

namespace EChamado.Server.Middlewares;

/// <summary>
/// Middleware para logar todas as requisições HTTP com detalhes
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        // Log da requisição
        _logger.LogInformation(
            "HTTP {Method} {Path} started - RequestId: {RequestId}, IP: {IP}, UserAgent: {UserAgent}",
            context.Request.Method,
            context.Request.Path,
            requestId,
            context.Connection.RemoteIpAddress,
            context.Request.Headers["User-Agent"].ToString());

        try
        {
            // Executa o próximo middleware
            await _next(context);

            stopwatch.Stop();

            // Log da resposta
            _logger.LogInformation(
                "HTTP {Method} {Path} completed - RequestId: {RequestId}, StatusCode: {StatusCode}, Duration: {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                requestId,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "HTTP {Method} {Path} failed - RequestId: {RequestId}, Duration: {Duration}ms, Error: {Error}",
                context.Request.Method,
                context.Request.Path,
                requestId,
                stopwatch.ElapsedMilliseconds,
                ex.Message);

            throw;
        }
    }
}
