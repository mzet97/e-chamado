using EChamado.Api.Configuration;
using EChamado.Api.Endpoints;
using EChamado.Api.Extensions;
using EChamado.Api.Middlewares;
using EChamado.Application.Configuration;
using EChamado.Infrastructure.Configuration;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
       .SetBasePath(builder.Environment.ContentRootPath)
       .AddJsonFile("appsettings.json", true, true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
       .AddEnvironmentVariables();

    builder.Services.AddRedisCache(builder.Configuration);
    builder.Services.AddRedisOutputCache(builder.Configuration);
    builder.Host.ConfigureSerilog(builder.Configuration);
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();

    builder.Services.AddIdentityConfig(builder.Configuration);
    builder.Services.AddCorsConfig();
    builder.Services.ResolveDependenciesInfrastructure();
    builder.Services.ResolveDependenciesApplication();

    builder.Services.AddMessageBus(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerConfig();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();
    builder.Services.AddAuthorization();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseCors("Development");
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseCors("Production");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();

    app.UseExceptionHandler(new ExceptionHandlerOptions
    {
        ExceptionHandler = async context =>
        {
            var exceptionHandler = new CustomExceptionHandler();
            var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null)
            {
                var exception = exceptionFeature.Error;
                if (!await exceptionHandler.TryHandleAsync(context, exception, CancellationToken.None))
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred.",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                    });
                }
            }
        }
    });

    app.UseMiddleware<RequestLoggingMiddleware>();
    app.UseCustomSerilogRequestLogging();

    app.UseAuthorization();
    app.UseHealthChecks("/health");
    app.UseStaticFiles();
    app.MapEndpoints();

    app.UseSwaggerConfig();

    app.MapGet("/cached-endpoint", async (HttpContext context) =>
    {
        context.Response.Headers["Cache-Control"] = "public, max-age=300";
        return Results.Ok(new { Message = "Este � um exemplo de cache", Timestamp = DateTime.UtcNow });
    }).CacheOutput("DefaultPolicy"); // Aplica a pol�tica de cache


    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

public partial class Program { }