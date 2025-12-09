using EChamado.Shared.Domain;

namespace EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;

public sealed record StatusTypeCreated(
    Guid StatusTypeId,
    string Name,
    string Description
) : DomainEvent;

public sealed record StatusTypeUpdated(
    Guid StatusTypeId,
    string Name,
    string Description
) : DomainEvent;