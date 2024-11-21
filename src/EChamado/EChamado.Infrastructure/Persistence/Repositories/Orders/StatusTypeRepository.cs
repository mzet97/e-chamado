using EChamado.Core.Domains.Orders.ValueObjects;
using EChamado.Core.Repositories.Orders;

namespace EChamado.Infrastructure.Persistence.Repositories.Orders;

public class StatusTypeRepository : Repository<StatusType>, IStatusTypeRepository
{
    public StatusTypeRepository(ApplicationDbContext db) : base(db)
    {
    }
}
