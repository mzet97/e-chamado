using EChamado.Server.Domain.Domains.Orders.Entities;

namespace EChamado.Server.Domain.Repositories.Orders;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
}
