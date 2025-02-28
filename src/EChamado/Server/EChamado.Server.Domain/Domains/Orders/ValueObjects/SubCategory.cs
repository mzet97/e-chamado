using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.ValueObjects;

public class SubCategory : ValueObject
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
