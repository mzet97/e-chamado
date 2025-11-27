using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories.Orders;

using EChamado.Shared.Services;
namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class OrderTypeRepository : Repository<OrderType>, IOrderTypeRepository
{
    public OrderTypeRepository(ApplicationDbContext db, IDateTimeProvider dateTimeProvider) : base(db, dateTimeProvider)
    {
    }
}
