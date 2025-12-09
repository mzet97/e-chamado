using EChamado.Shared.Domain;

namespace EChamado.Server.Domain.Domains.Orders.Events.Orders;

public sealed record OrderCreated(
    Guid Id,
    string Title,
    string Description,
    Guid StatusId,
    Guid TypeId,
    Guid CategoryId,
    Guid DepartmentId,
    Guid? SubCategoryId,
    Guid RequestingUserId,
    string RequestingUserEmail,
    Guid ResponsibleUserId,
    string ResponsibleUserEmail,
    DateTime OpeningDateUtc,
    DateTime? DueDateUtc
) : DomainEvent;

public sealed record OrderUpdated(
    Guid Id,
    string Title,
    string Description,
    Guid StatusId,
    Guid TypeId,
    Guid CategoryId,
    Guid DepartmentId,
    Guid? SubCategoryId,
    Guid ResponsibleUserId,
    string ResponsibleUserEmail,
    DateTime? DueDateUtc
) : DomainEvent;

public sealed record OrderClosed(
    Guid Id,
    DateTime ClosingDateUtc,
    string? Evaluation
) : DomainEvent;