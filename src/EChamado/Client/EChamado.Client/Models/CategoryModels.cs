namespace EChamado.Client.Models;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    List<SubCategoryResponse> SubCategories
);

public record SubCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId
);

public record CreateCategoryRequest(string Name, string Description);
public record UpdateCategoryRequest(string Name, string Description);
public record CreateSubCategoryRequest(string Name, string Description);
public record UpdateSubCategoryRequest(string Name, string Description);
