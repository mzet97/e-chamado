using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Services;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
{
    public SubCategoryRepository(ApplicationDbContext db, IDateTimeProvider dateTimeProvider) : base(db, dateTimeProvider)
    {
    }
}
