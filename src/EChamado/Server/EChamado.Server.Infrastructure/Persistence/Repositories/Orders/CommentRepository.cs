using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext db) : base(db)
    {
    }

    public async Task<IEnumerable<Comment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.OrderId == orderId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
