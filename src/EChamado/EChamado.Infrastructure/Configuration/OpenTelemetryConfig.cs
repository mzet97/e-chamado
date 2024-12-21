using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry;

public static class OpenTelemetryConfig
{
    public static IServiceCollection AddOpenTelemetryConfig(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var serviceName = configuration.GetValue<string>("OpenTelemetry:ServiceName", "EChamado.Api");
        var otlpEndpoint = configuration.GetValue<string>("OpenTelemetry:OtlpEndpoint", "http://172.18.170.12:4317");

        services.AddOpenTelemetry()
          .WithTracing(trace => ConfigureTracing(trace, serviceName, otlpEndpoint))
          .WithMetrics(metrics => ConfigureMetrics(metrics, serviceName, otlpEndpoint));

        return services;
    }

    public static ILoggingBuilder AddOpenTelemetryConfig(this ILoggingBuilder logging)
    {
        var configuration = logging.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var serviceName = configuration.GetValue<string>("OpenTelemetry:ServiceName", "EChamado.Api");
        var otlpEndpoint = configuration.GetValue<string>("OpenTelemetry:OtlpEndpoint", "http://172.18.170.12:4317");

        logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.IncludeFormattedMessage = true;
            options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
            options.AddProcessor(new SimpleLogRecordExportProcessor(new OtlpLogExporter(new OtlpExporterOptions
            {
                Endpoint = new Uri(otlpEndpoint),
                //Protocol = OtlpExportProtocol.HttpProtobuf
                Protocol = OtlpExportProtocol.Grpc
            })));
        });

        return logging;
    }

    private static TracerProviderBuilder ConfigureTracing(TracerProviderBuilder builder, string serviceName, string otlpEndpoint)
    {
        return builder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
                //options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.Protocol = OtlpExportProtocol.Grpc;
            });
    }

    private static MeterProviderBuilder ConfigureMetrics(MeterProviderBuilder builder, string serviceName, string otlpEndpoint)
    {
        return builder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
                //options.Protocol = OtlpExportProtocol.HttpProtobuf;
                options.Protocol = OtlpExportProtocol.Grpc;
            });
    }
}
