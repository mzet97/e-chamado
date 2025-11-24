using EChamado.Server.Application.Configuration;
using EChamado.Server.Application.Users;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Application.Users.Handlers;
using EChamado.Server.Application.Users.Queries;
using EChamado.Server.Configuration;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Server.Endpoints;
using EChamado.Server.Infrastructure.Configuration;
using EChamado.Server.Infrastructure.MessageBus;
using EChamado.Server.Infrastructure.Users;
using EChamado.Server.Middlewares;
using EChamado.Server.Presentation.Api.Endpoints;
using Scrutor;
using Serilog;

try
{
    Log.Information("=== Starting EChamado Server ===");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.ConfigureSerilog(builder.Configuration);

    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowBlazorClient", policy =>
        {
            policy.WithOrigins("https://localhost:7274", "https://localhost:7133")
                  .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
    });

    // Core services
    builder.Services.AddIdentityConfig(builder.Configuration);
    builder.Services.AddMemoryCache();
    builder.Services.AddDistributedMemoryCache();
    
    // Optional services (Redis/MessageBus) - usar fallbacks sem try/catch
    builder.Services.AddRedisCache(builder.Configuration);
    builder.Services.AddRedisOutputCache(builder.Configuration);
    builder.Services.AddMessageBus(builder.Configuration);

    // Application & Infrastructure
    builder.Services.AddScoped<IUserReadRepository, EfUserReadRepository>();
    builder.Services.AddApplicationServices();
    builder.Services.ResolveDependenciesApplication();
    builder.Services.ResolveDependenciesInfrastructure();
    builder.Services.AddApiDocumentation();
    builder.Services.AddHealthCheckConfiguration(builder.Configuration);
    builder.Services.AddControllers();

    var app = builder.Build();

    // Database initialization
    await DatabaseInitializer.InitializeDatabaseAsync(app.Services);

    // Middleware pipeline (ordem correta é CRÍTICA!)
    app.UseCors("AllowBlazorClient");
    app.UseRequestLogging();
    app.UsePerformanceLogging(slowRequestThresholdMs: 3000);
    app.UseApiDocumentation();
    app.UseHealthCheckConfiguration();

    // ✅ ORDEM CORRETA: Routing → Authentication → Authorization → Endpoints
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    // Endpoints
    app.MapEndpoints();
    app.MapUserReadEndpoints();
    app.MapControllers();
    app.MapGet("/swagger", () => Results.Redirect("/api-docs/v1", permanent: true));
    app.MapGet("/swagger/index.html", () => Results.Redirect("/api-docs/v1", permanent: true));

    Log.Information("=== EChamado Server configured successfully, starting... ===");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "=== Application terminated unexpectedly ===");
    return 1;
}
finally
{
    Log.Information("=== Shutting down EChamado Server ===");
    Log.CloseAndFlush();
}

return 0;

public partial class Program { }

