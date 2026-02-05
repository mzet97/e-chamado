using EChamado.Server.Domain.Services.Interface;
using EChamado.Server.Infrastructure.MessageBus;
using EChamado.Shared.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EChamado.Server.Infrastructure.Configuration;

public static class MessageBusConfig
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            var rabbitMqSection = configuration.GetSection("RabbitMQ");

            if (!rabbitMqSection.Exists())
            {
                services.AddSingleton<IMessageBusClient, NullMessageBusClient>();
                return services;
            }

            services.Configure<RabbitMq>(rabbitMqSection);
            var rabbitMq = rabbitMqSection.Get<RabbitMq>();

            if (rabbitMq == null || string.IsNullOrEmpty(rabbitMq.HostName))
            {
                services.AddSingleton<IMessageBusClient, NullMessageBusClient>();
                return services;
            }

            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMq.HostName,
                Port = rabbitMq.Port,
                UserName = rabbitMq.Username,
                Password = rabbitMq.Password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                SocketReadTimeout = TimeSpan.FromSeconds(30),
                SocketWriteTimeout = TimeSpan.FromSeconds(30)
            };

            services.AddSingleton(async serviceProvider =>
            {
                var logger = serviceProvider.GetService<ILogger<IConnection>>();
                try
                {
                    var connection = await connectionFactory.CreateConnectionAsync(rabbitMq.ClientProviderName);
                    logger?.LogInformation("Successfully connected to RabbitMQ at {HostName}:{Port}", rabbitMq.HostName, rabbitMq.Port);
                    return connection;
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Failed to connect to RabbitMQ at {HostName}:{Port}", rabbitMq.HostName, rabbitMq.Port);
                    throw;
                }
            });

            services.AddSingleton<ProducerConnection?>(serviceProvider =>
            {
                try
                {
                    var connectionTask = serviceProvider.GetRequiredService<Task<IConnection>>();
                    var connection = connectionTask.GetAwaiter().GetResult();
                    return new ProducerConnection(connection);
                }
                catch
                {
                    // If connection fails, return null
                    return null;
                }
            });

            services.AddSingleton<IMessageBusClient>(serviceProvider =>
            {
                try
                {
                    var producerConnection = serviceProvider.GetRequiredService<ProducerConnection?>();
                    if (producerConnection?.Connection != null)
                    {
                        return new RabbitMqClient(producerConnection);
                    }
                }
                catch
                {
                    // Fall through to null client
                }

                return serviceProvider.GetRequiredService<NullMessageBusClient>();
            });

            // Always register the null client as a fallback
            services.AddSingleton<NullMessageBusClient>();
        }
        catch (Exception ex)
        {
            // If anything fails, use null message bus
            using var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("MessageBusConfig");
            logger?.LogWarning(ex, "Failed to configure RabbitMQ, using null message bus client");
            services.AddSingleton<IMessageBusClient, NullMessageBusClient>();
        }

        return services;
    }
}

