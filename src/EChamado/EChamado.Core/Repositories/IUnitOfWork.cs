using EChamado.Core.Shared;

namespace EChamado.Core.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> Repository<TEntity>() where TEntity : IEntity;

    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();

    Task<int> SaveChangesAsync();
}
