using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.SubCategories;

public class SubCategoryCreated : DomainEvent
{
    public SubCategory SubCategory { get; }

    public SubCategoryCreated(SubCategory subCategory)
    {
        SubCategory = subCategory;
    }
}