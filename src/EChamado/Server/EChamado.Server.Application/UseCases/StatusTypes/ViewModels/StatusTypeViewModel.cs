namespace EChamado.Server.Application.UseCases.StatusTypes.ViewModels;

public record StatusTypeViewModel(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeletedAtUtc,
    bool IsDeleted
);
