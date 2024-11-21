using EChamado.Core.Shared;

namespace EChamado.Core.Domains.Orders.ValueObjects;

public class OrderType : ValueObject
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
