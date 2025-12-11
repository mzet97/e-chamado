namespace EChamado.Server.Domain.Services.Interface;

public interface IMessageBusClient
{
    Task Publish(
        object message,
        string routingKey,
        string exchange,
        string type,
        string queueName);

    Task Subscribe(
        string queueName,
        string exchange,
        string type,
        string routingKey,
        Action<string> onMessageReceived);
}
