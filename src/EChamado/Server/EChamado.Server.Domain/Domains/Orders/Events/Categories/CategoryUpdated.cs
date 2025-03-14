using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Categories;

public class CategoryUpdated : DomainEvent
{
    public Category Category { get; }

    public CategoryUpdated(Category category)
    {
        Category = category;
    }
}

