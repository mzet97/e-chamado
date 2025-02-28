using EChamado.Server.Domain.Domains.Orders.ValueObjects;
using EChamado.Server.Domain.Repositories.Orders;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class OrderTypeRepository : Repository<OrderType>, IOrderTypeRepository
{
    public OrderTypeRepository(ApplicationDbContext db) : base(db)
    {
    }
}
