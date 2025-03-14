using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;

public class StatusTypeCreated : DomainEvent
{
    public StatusType StatusType { get; }

    public StatusTypeCreated(StatusType statusType)
    {
        StatusType = statusType;
    }
}
