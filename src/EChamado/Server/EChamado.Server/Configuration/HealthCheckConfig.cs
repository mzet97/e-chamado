using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace EChamado.Server.Configuration;

public static class HealthCheckConfig
{
    public static IServiceCollection AddHealthCheckConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var redisConnection = configuration["Redis:ConnectionString"];

        services.AddHealthChecks()
            // PostgreSQL Health Check
            .AddNpgSql(
                connectionString!,
                name: "postgresql",
                tags: new[] { "db", "sql", "postgresql" })

            // Redis Health Check
            .AddRedis(
                redisConnection!,
                name: "redis",
                tags: new[] { "cache", "redis" });

        // Health Checks UI
        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(30); // Avalia a cada 30 segundos
            setup.MaximumHistoryEntriesPerEndpoint(50);
            setup.AddHealthCheckEndpoint("EChamado API", "/health");
        })
        .AddInMemoryStorage();

        return services;
    }

    public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
    {
        // Endpoint principal de health checks (JSON)
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
            {
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status200OK,
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        // Kubernetes Readiness Probe
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("cache"),
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            ResultStatusCodes =
            {
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy] = StatusCodes.Status200OK,
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                [Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        // Kubernetes Liveness Probe
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false, // Apenas verifica se a aplicação está respondendo
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        // Health Checks UI Dashboard
        app.UseHealthChecksUI(config =>
        {
            config.UIPath = "/health-ui";
            config.ApiPath = "/health-ui-api";
        });

        return app;
    }
}
