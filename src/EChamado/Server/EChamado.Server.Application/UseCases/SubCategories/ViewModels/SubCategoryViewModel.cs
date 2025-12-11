using EChamado.Shared.ViewModels;

namespace EChamado.Server.Application.UseCases.SubCategories.ViewModels;

public record SubCategoryViewModel(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeletedAtUtc,
    bool IsDeleted
);
