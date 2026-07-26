using EChamado.Server.Domain.Domains.Orders.Entities;

namespace EChamado.Server.Domain.Repositories.Orders;

public interface IAttachmentRepository : IRepository<Attachment>
{
    Task<IEnumerable<Attachment>> GetByCommentIdAsync(Guid commentId, CancellationToken cancellationToken = default);
}
