using EChamado.Core.Domains.Orders.ValueObjects;
using EChamado.Core.Repositories.Orders;

namespace EChamado.Infrastructure.Persistence.Repositories.Orders;

public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
{
    public SubCategoryRepository(ApplicationDbContext db) : base(db)
    {
    }
}
