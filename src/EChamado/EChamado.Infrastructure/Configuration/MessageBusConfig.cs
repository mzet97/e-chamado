using EChamado.Core.Shared.Settings;
using EChamado.Infrastructure.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EChamado.Infrastructure.Configuration;

public static class MessageBusConfig
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionFactory = new ConnectionFactory();

        var rabbitMqSection = configuration.GetSection("RabbitMq");
        services.Configure<RabbitMq>(rabbitMqSection);
        var rabbitMq = rabbitMqSection.Get<RabbitMq>();

        if (rabbitMq == null)
        {
            throw new ArgumentNullException("RabbitMq configuration is missing");
        }

        connectionFactory.HostName = rabbitMq.HostName;
        connectionFactory.Port = rabbitMq.Port;
        connectionFactory.UserName = rabbitMq.Username;
        connectionFactory.Password = rabbitMq.Password;

        services.AddSingleton(async serviceProvider =>
        {
            var connection = await connectionFactory.CreateConnectionAsync(rabbitMq.ClientProviderName);
            return new ProducerConnection(connection);
        });

        services.AddSingleton<IMessageBusClient>(serviceProvider =>
        {
            var producerConnectionTask = serviceProvider.GetRequiredService<Func<Task<ProducerConnection>>>();
            var producerConnection = producerConnectionTask().GetAwaiter().GetResult();
            return new RabbitMqClient(producerConnection);
        });

        return services;
    }
}


