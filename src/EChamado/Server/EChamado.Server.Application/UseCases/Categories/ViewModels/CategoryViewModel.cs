namespace EChamado.Server.Application.UseCases.Categories.ViewModels;

public record CategoryViewModel(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    DateTime? DeletedAtUtc,
    bool IsDeleted,
    List<SubCategoryViewModel> SubCategories
);

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
