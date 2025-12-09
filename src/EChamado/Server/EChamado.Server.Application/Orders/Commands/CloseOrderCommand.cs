using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Commands;

public sealed class CloseOrderCommand : Command
{
    public Guid OrderId { get; set; }
    public int Evaluation { get; set; }

    public CloseOrderCommand() : base(Guid.NewGuid())
    {
    }
}
