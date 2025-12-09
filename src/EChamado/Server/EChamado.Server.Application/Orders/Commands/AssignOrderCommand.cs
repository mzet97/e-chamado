using Paramore.Brighter;

namespace EChamado.Server.Application.Orders.Commands;

public sealed class AssignOrderCommand : Command
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;

    public AssignOrderCommand() : base(Guid.NewGuid())
    {
    }
}
