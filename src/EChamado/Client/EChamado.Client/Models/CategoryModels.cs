namespace EChamado.Client.Models;

public record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    List<SubCategoryResponse> SubCategories
);

public class SubCategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }

    public SubCategoryResponse(Guid id, string name, string description, Guid categoryId, string? categoryName = null)
    {
        Id = id;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        CategoryName = categoryName;
    }
}

public record CreateCategoryRequest(string Name, string Description);
public record UpdateCategoryRequest(string Name, string Description);
public record CreateSubCategoryRequest(string Name, string Description, Guid CategoryId);
public record UpdateSubCategoryRequest(string Name, string Description, Guid CategoryId);
