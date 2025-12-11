using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Commands;

public sealed class ChangeOrderStatusCommand : Command
{
    public Guid OrderId { get; set; }
    public Guid StatusId { get; set; }

    public ChangeOrderStatusCommand() : base(Guid.NewGuid())
    {
    }
}
