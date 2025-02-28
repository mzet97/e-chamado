using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Infrastructure.Persistence;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext db) : base(db)
    {
    }
}
