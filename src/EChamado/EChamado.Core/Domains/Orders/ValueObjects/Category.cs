using EChamado.Core.Shared;

namespace EChamado.Core.Domains.Orders.ValueObjects;

public class Category : ValueObject
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public IEnumerable<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
