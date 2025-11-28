using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Services;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext db, IDateTimeProvider dateTimeProvider) : base(db, dateTimeProvider)
    {
    }
}
