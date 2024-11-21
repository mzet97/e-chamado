using EChamado.Core.Repositories;
using EChamado.Core.Shared;
using Microsoft.EntityFrameworkCore.Storage;

namespace EChamado.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IRepository<TEntity> Repository<TEntity>() where TEntity : IEntity
    {
        if (!_repositories.ContainsKey(typeof(TEntity)))
        {
            var repositoryType = typeof(Repository<>).MakeGenericType(typeof(TEntity));
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(typeof(TEntity), repositoryInstance!);
        }

        return (IRepository<TEntity>)_repositories[typeof(TEntity)];
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction == null)
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync();
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}