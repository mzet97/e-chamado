using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Events.SubCategories;

public class SubCategoryUpdated : DomainEvent
{
    public SubCategory SubCategory { get; }

    public SubCategoryUpdated(SubCategory subCategory)
    {
        SubCategory = subCategory;
    }
}