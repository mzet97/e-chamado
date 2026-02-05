namespace EChamado.Server.Application.UseCases.OrderTypes.ViewModels;

public record OrderTypeViewModel(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeletedAtUtc,
    bool IsDeleted
);
