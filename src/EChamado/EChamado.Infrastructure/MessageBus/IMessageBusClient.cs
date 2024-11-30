namespace EChamado.Infrastructure.MessageBus;

public interface IMessageBusClient
{
    void Publish(
        object message,
        string routingKey,
        string exchange,
        string type,
        string queueName);

    void Subscribe(
        string queueName,
        string exchange,
        string type,
        string routingKey,
        Action<string> onMessageReceived);
}

