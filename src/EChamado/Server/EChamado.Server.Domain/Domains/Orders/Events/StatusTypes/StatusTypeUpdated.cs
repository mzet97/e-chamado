using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;

public class StatusTypeUpdated : DomainEvent
{
    public StatusType StatusType { get; }

    public StatusTypeUpdated(StatusType statusType)
    {
        StatusType = statusType;
    }
}
