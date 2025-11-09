namespace EChamado.Server.Middlewares;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware de logging de requisições
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }

    /// <summary>
    /// Adiciona o middleware de logging de performance
    /// </summary>
    /// <param name="slowRequestThresholdMs">Tempo limite em ms para considerar uma requisição lenta (padrão: 3000ms)</param>
    public static IApplicationBuilder UsePerformanceLogging(
        this IApplicationBuilder builder,
        int slowRequestThresholdMs = 3000)
    {
        return builder.UseMiddleware<PerformanceLoggingMiddleware>(slowRequestThresholdMs);
    }
}
