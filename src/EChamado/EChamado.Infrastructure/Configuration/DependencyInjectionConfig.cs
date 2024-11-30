using EChamado.Core.Repositories;
using EChamado.Core.Repositories.Orders;
using EChamado.Infrastructure.Persistence;
using EChamado.Infrastructure.Persistence.Repositories;
using EChamado.Infrastructure.Persistence.Repositories.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EChamado.Infrastructure.Configuration;

public static class DependencyInjectionConfig
{
    public static IServiceCollection ResolveDependenciesInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<DbContext, ApplicationDbContext>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderTypeRepository, OrderTypeRepository>();
        services.AddScoped<IStatusTypeRepository, StatusTypeRepository>();
        services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();

        return services;
    }
}
