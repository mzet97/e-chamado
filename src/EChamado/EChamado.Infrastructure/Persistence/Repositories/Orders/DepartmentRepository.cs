using EChamado.Core.Domains.Orders.ValueObjects;
using EChamado.Core.Repositories.Orders;

namespace EChamado.Infrastructure.Persistence.Repositories.Orders;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext db) : base(db)
    {
    }
}
