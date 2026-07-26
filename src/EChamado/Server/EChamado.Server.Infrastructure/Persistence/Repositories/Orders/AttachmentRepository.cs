using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Infrastructure.Persistence.Repositories.Orders;

public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
{
    public AttachmentRepository(ApplicationDbContext db, IDateTimeProvider dateTimeProvider)
        : base(db, dateTimeProvider) { }

    public async Task<IEnumerable<Attachment>> GetByCommentIdAsync(
        Guid commentId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(a => a.CommentId == commentId && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}
