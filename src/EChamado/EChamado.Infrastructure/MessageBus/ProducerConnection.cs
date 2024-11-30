using RabbitMQ.Client;

namespace EChamado.Infrastructure.MessageBus;

public class ProducerConnection
{
    public IConnection Connection { get; private set; }

    public ProducerConnection(IConnection connection)
    {
        Connection = connection;
    }
}
