using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace EChamado.Server.Configuration;

public static class SerilogConfig
{
    public static void ConfigureSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        var elasticUri = configuration["ElasticSettings:Uri"];
        var elasticEnabled = string.Equals(configuration["ElasticSettings:Enabled"], "true", StringComparison.OrdinalIgnoreCase);
        var username = configuration["ElasticSettings:Username"];
        var password = configuration["ElasticSettings:Password"];

        // Habilita SelfLog para diagnosticar erros internos do Serilog
        SelfLog.Enable(msg => Console.WriteLine($"[Serilog SelfLog] {msg}"));

        builder.UseSerilog((ctx, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(ctx.Configuration)
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug);

            // Sink Elasticsearch (Serilog.Sinks.Elasticsearch)
            if (elasticEnabled && !string.IsNullOrEmpty(elasticUri))
            {
                try
                {
                    var options = new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        IndexFormat = "echamado-logs-{0:yyyy.MM.dd}",
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                        OverwriteTemplate = true,
                        ModifyConnectionSettings = conn =>
                        {
                            if (ctx.HostingEnvironment.IsDevelopment())
                                conn.ServerCertificateValidationCallback((_, _, _, _) => true);

                            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                                conn.BasicAuthentication(username, password);

                            return conn;
                        }
                    };

                    loggerConfig.WriteTo.Elasticsearch(options);

                    Console.WriteLine($"[Serilog] Elasticsearch sink configurado: {elasticUri}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Serilog] Falha ao configurar Elasticsearch sink: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[Serilog] Elasticsearch desabilitado (Enabled={elasticEnabled}, Uri={elasticUri})");
            }
        });
    }
}
