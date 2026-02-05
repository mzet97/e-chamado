using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Domain;
using EChamado.Shared.Responses;
using EChamado.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EChamado.Server.Infrastructure.Persistence.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly ApplicationDbContext Db;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly IDateTimeProvider DateTimeProvider;

    protected Repository(ApplicationDbContext db, IDateTimeProvider dateTimeProvider)
    {
        Db = db ?? throw new ArgumentNullException(nameof(db));
        DateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        DbSet = db.Set<TEntity>();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        // opcional: garantir auditoria caso algo escape na factory
        if (entity is IAuditable auditable)
            auditable.MarkCreated(DateTimeProvider.UtcNow);

        await DbSet.AddAsync(entity);
        await Db.SaveChangesAsync();
    }

    public virtual async Task<BaseResultList<TEntity>> SearchAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        int pageSize = 10, int page = 1)
    {
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));

        var query = DbSet.AsNoTracking().AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync();
        var paged = PagedResult.Create(page, pageSize, totalCount);

        if (orderBy != null)
            query = orderBy(query);

        var data = await query.Skip(paged.Skip()).Take(pageSize).ToListAsync();
        return new BaseResultList<TEntity>(data, paged);
    }

    public virtual async Task<BaseResultList<TEntity>> SearchAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "",
        int pageSize = 10, int page = 1)
    {
        if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
        if (page <= 0) throw new ArgumentOutOfRangeException(nameof(page));

        var query = DbSet.AsNoTracking().AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync();
        var paged = PagedResult.Create(page, pageSize, totalCount);

        if (orderBy != null)
            query = orderBy(query);

        foreach (var includeProperty in includeProperties
                     .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        var data = await query.Skip(paged.Skip()).Take(pageSize).ToListAsync();
        return new BaseResultList<TEntity>(data, paged);
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        => await DbSet.AsNoTracking().ToListAsync();

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID não pode ser vazio", nameof(id));
        return await DbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        if (entity is IAuditable auditable)
            auditable.MarkUpdated(DateTimeProvider.UtcNow);

        DbSet.Update(entity);
        await Db.SaveChangesAsync();
    }

    public virtual async Task RemoveAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID não pode ser vazio", nameof(id));

        var entity = await DbSet.FindAsync(id);

        if (entity == null)
            throw new InvalidOperationException("Entidade não encontrada para exclusão.");

        DbSet.Remove(entity);
        await Db.SaveChangesAsync();
    }

    // NOVO: equivalente do Disable/Activate legacy
    public virtual async Task DisableAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID não pode ser vazio", nameof(id));

        var entity = await DbSet.FindAsync(id);
        if (entity == null) return;

        if (entity is ISoftDeletable soft)
        {
            soft.SoftDelete(DateTimeProvider.UtcNow);
            DbSet.Update(entity);
            await Db.SaveChangesAsync();
            return;
        }

        // fallback: se não for soft-deletable, remove fisicamente
        DbSet.Remove(entity);
        await Db.SaveChangesAsync();
    }

    public virtual async Task ActiveAsync(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("ID não pode ser vazio", nameof(id));

        var entity = await DbSet.FindAsync(id);
        if (entity == null) return;

        if (entity is ISoftDeletable soft)
        {
            soft.Restore();
            DbSet.Update(entity);
            await Db.SaveChangesAsync();
        }
    }

    public Task ActiveOrDisableAsync(Guid id, bool active)
        => active ? ActiveAsync(id) : DisableAsync(id);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        => predicate == null ? await DbSet.CountAsync() : await DbSet.CountAsync(predicate);

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return await DbSet.AsNoTracking().AnyAsync(predicate);
    }

    public virtual IQueryable<TEntity> GetAllQueryable()
        => DbSet.AsNoTracking().AsQueryable();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            Db?.Dispose();
    }
}