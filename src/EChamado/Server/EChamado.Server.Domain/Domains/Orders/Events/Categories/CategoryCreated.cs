using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.Categories;

public class CategoryCreated : DomainEvent
{
    public Category Category { get; }

    public CategoryCreated(Category category)
    {
        Category = category;
    }
}
