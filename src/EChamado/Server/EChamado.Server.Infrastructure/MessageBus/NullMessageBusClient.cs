using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Infrastructure.MessageBus;

/// <summary>
/// Null implementation of IMessageBusClient for development/testing without RabbitMQ.
/// This implementation does nothing and logs warnings when methods are called.
/// </summary>
public class NullMessageBusClient : IMessageBusClient
{
    private readonly ILogger<NullMessageBusClient> _logger;

    public NullMessageBusClient(ILogger<NullMessageBusClient> logger)
    {
        _logger = logger;
    }

    public Task Publish(
        object message,
        string routingKey,
        string exchange,
        string type,
        string queueName)
    {
        _logger.LogWarning(
            "NullMessageBusClient: Publish called but RabbitMQ is not configured. " +
            "Message type: {MessageType}, RoutingKey: {RoutingKey}, Exchange: {Exchange}",
            message.GetType().Name,
            routingKey,
            exchange);

        return Task.CompletedTask;
    }

    public Task Subscribe(
        string queueName,
        string exchange,
        string type,
        string routingKey,
        Action<string> onMessageReceived)
    {
        _logger.LogWarning(
            "NullMessageBusClient: Subscribe called but RabbitMQ is not configured. " +
            "QueueName: {QueueName}, Exchange: {Exchange}, RoutingKey: {RoutingKey}",
            queueName,
            exchange,
            routingKey);

        return Task.CompletedTask;
    }
}
