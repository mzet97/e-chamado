using Microsoft.AspNetCore.Builder;
using Serilog;

namespace EChamado.Server.Middlewares;

public static class SerilogMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomSerilogRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme ?? string.Empty);
                diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? string.Empty);
                diagnosticContext.Set("RequestQuery", httpContext.Request.QueryString.Value ?? string.Empty);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString() ?? string.Empty);
            };

            options.GetLevel = (httpContext, elapsed, ex) =>
            {
                // Custom logic for determining log level
                if (ex != null || httpContext.Response.StatusCode >= 500)
                {
                    return Serilog.Events.LogEventLevel.Error;
                }

                if (elapsed > 1000)
                {
                    return Serilog.Events.LogEventLevel.Warning;
                }

                return Serilog.Events.LogEventLevel.Information;
            };
        });
    }
}
