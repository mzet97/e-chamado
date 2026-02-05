using EChamado.Shared.Domain;

namespace EChamado.Server.Domain.Domains.Orders.Events.Departments;

public sealed record DepartmentCreated(
    Guid DepartmentId,
    string Name,
    string Description
) : DomainEvent;

public sealed record DepartmentUpdated(
    Guid DepartmentId,
    string Name,
    string Description
) : DomainEvent;