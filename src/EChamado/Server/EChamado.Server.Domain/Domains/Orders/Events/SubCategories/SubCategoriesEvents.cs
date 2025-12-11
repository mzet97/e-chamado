using EChamado.Shared.Domain;

namespace EChamado.Server.Domain.Domains.Orders.Events.SubCategories;

public sealed record SubCategoryCreated(
    Guid SubCategoryId,
    Guid CategoryId,
    string Name,
    string Description
) : DomainEvent;

public sealed record SubCategoryUpdated(
    Guid SubCategoryId,
    Guid CategoryId,
    string Name,
    string Description
) : DomainEvent;