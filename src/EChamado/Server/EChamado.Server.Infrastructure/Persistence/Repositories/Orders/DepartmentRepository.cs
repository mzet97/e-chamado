using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Services;
using EChamado.Shared.Services;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext db, IDateTimeProvider dateTimeProvider) : base(db, dateTimeProvider)
    {
    }
}
