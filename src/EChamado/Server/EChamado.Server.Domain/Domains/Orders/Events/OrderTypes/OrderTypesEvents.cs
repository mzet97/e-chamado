using EChamado.Shared.Domain;

namespace EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;

public sealed record OrderTypeCreated(
    Guid OrderTypeId,
    string Name,
    string Description
) : DomainEvent;

public sealed record OrderTypeUpdated(
    Guid OrderTypeId,
    string Name,
    string Description
) : DomainEvent;