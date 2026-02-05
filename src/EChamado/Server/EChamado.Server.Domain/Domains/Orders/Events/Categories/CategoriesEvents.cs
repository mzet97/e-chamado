using EChamado.Shared.Domain;

namespace EChamado.Server.Domain.Domains.Orders.Events.Categories;

public sealed record CategoryCreated(
    Guid CategoryId,
    string Name,
    string Description
) : DomainEvent;

public sealed record CategoryUpdated(
    Guid CategoryId,
    string Name,
    string Description
) : DomainEvent;
