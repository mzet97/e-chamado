using EChamado.Server.Infrastructure.Persistence.Repositories.Orders;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Services;

namespace EChamado.Server.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnitOfWork(ApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    private ICategoryRepository? _categoryRepository;
    public ICategoryRepository Categories =>
        _categoryRepository ??= new CategoryRepository(_context, _dateTimeProvider);

    private ICommentRepository? _commentsRepository;
    public ICommentRepository Comments =>
        _commentsRepository ??= new CommentRepository(_context, _dateTimeProvider);

    private IDepartmentRepository? _departmentsRepository;
    public IDepartmentRepository Departments =>
        _departmentsRepository ??= new DepartmentRepository(_context, _dateTimeProvider);

    private IOrderRepository? _ordersRepository;
    public IOrderRepository Orders =>
        _ordersRepository ??= new OrderRepository(_context, _dateTimeProvider);

    private IOrderTypeRepository? _orderTypesRepository;
    public IOrderTypeRepository OrderTypes =>
        _orderTypesRepository ??= new OrderTypeRepository(_context, _dateTimeProvider);

    private IStatusTypeRepository? _statusTypesRepository;
    public IStatusTypeRepository StatusTypes =>
        _statusTypesRepository ??= new StatusTypeRepository(_context, _dateTimeProvider);

    private ISubCategoryRepository? _subCategoriesRepository;
    public ISubCategoryRepository SubCategories =>
        _subCategoriesRepository ??= new SubCategoryRepository(_context, _dateTimeProvider);


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
