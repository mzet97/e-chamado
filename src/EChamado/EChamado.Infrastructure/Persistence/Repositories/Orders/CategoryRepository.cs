using EChamado.Core.Domains.Orders.ValueObjects;
using EChamado.Core.Repositories.Orders;

namespace EChamado.Infrastructure.Persistence.Repositories.Orders;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
    }
}
