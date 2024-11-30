using EChamado.Core.Repositories.Orders;

namespace EChamado.Core.Repositories;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
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
