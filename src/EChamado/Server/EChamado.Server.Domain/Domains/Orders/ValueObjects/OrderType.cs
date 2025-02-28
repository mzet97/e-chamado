using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.ValueObjects;

public class OrderType : ValueObject
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
