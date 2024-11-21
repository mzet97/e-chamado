using EChamado.Core.Domains.Orders;
using EChamado.Core.Repositories.Orders;

namespace EChamado.Infrastructure.Persistence.Repositories.Orders;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext db) : base(db)
    {
    }
}
