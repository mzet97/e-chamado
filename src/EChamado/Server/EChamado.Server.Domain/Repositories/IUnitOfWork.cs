using EChamado.Server.Domain.Repositories.Orders;

namespace EChamado.Server.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    ICommentRepository Comments { get; }
    IDepartmentRepository Departments { get; }
    IOrderRepository Orders { get; }
    IOrderTypeRepository OrderTypes { get; }
    IStatusTypeRepository StatusTypes { get; }
    ISubCategoryRepository SubCategories { get; }

    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();

    Task<int> SaveChangesAsync();
}
