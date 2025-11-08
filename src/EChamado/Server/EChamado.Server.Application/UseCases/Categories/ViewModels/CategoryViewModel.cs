namespace EChamado.Server.Application.UseCases.Categories.ViewModels;

public record CategoryViewModel(
    Guid Id,
    string Name,
    string Description,
    List<SubCategoryViewModel> SubCategories
);

public record SubCategoryViewModel(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId
);
