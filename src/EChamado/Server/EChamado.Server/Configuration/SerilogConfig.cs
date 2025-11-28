using Serilog;

namespace EChamado.Server.Configuration;

public static class SerilogConfig
{
    public static void ConfigureSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        builder.UseSerilog((ctx, loggerConfig) =>
        {
            loggerConfig
                // Configurações do Serilog são lidas diretamente do appsettings.json
                // A seção "Serilog" no appsettings.json contém toda a configuração
                // incluindo o sink do Elasticsearch com o indexFormat: "logs-echamado-{0:yyyy.MM.dd}"
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Information)
                // Console para logs locais + Elasticsearch para centralização via appsettings.json
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug);
        });
    }
}
