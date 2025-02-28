using EChamado.Server.Infrastructure.Persistence.Repositories.Orders;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.Domain.Repositories.Orders;

namespace EChamado.Server.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    private ICategoryRepository? _categoryRepository;
    public ICategoryRepository Categories => 
        _categoryRepository ??= new CategoryRepository(_context);

    private IDepartmentRepository? _departmentsRepository;
    public IDepartmentRepository Departments =>
        _departmentsRepository ??= new DepartmentRepository(_context);

    private IOrderRepository? _ordersRepository;
    public IOrderRepository Orders =>
        _ordersRepository ??= new OrderRepository(_context);

    private IOrderTypeRepository? _orderTypesRepository;
    public IOrderTypeRepository OrderTypes =>
        _orderTypesRepository ??= new OrderTypeRepository(_context);

    private IStatusTypeRepository? _statusTypesRepository;
    public IStatusTypeRepository StatusTypes =>
        _statusTypesRepository ??= new StatusTypeRepository(_context);

    private ISubCategoryRepository? _subCategoriesRepository;
    public ISubCategoryRepository SubCategories =>
        _subCategoriesRepository ??= new SubCategoryRepository(_context);


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        if (_context.Database.CurrentTransaction == null)
        {
            await _context.Database.BeginTransactionAsync();
        }
    }

    public async Task CommitAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
        }
    }

    public async Task RollbackAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
