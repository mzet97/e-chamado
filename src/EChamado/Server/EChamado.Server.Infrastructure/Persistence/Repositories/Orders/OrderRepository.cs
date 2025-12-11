using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext db, IDateTimeProvider dateTimeProvider) : base(db, dateTimeProvider)
    {
    }

    public async Task<Order?> GetByIdWithIncludesAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID não pode ser vazio", nameof(id));

        return await DbSet
            .AsNoTracking()
            .Include(o => o.Status)
            .Include(o => o.Type)
            .Include(o => o.Category)
            .Include(o => o.SubCategory)
            .Include(o => o.Department)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
}
