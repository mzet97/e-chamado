using RabbitMQ.Client;

namespace EChamado.Server.Infrastructure.MessageBus;

public class ProducerConnection
{
    public IConnection Connection { get; private set; }

    public ProducerConnection(IConnection connection)
    {
        Connection = connection;
    }
}
