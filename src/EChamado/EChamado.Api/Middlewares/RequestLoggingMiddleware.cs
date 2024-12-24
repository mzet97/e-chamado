using Serilog;

namespace EChamado.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();

        if (context.Request.Body == null) {
            await _next(context);
            return;
        }

        if(context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }
        if(context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        Log.Information("HTTP Request Information: {Method} {Path} {Body}", context.Request.Method, context.Request.Path, body);

        await _next(context);
    }
}

