using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Server.Infrastructure.Email;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Server.Infrastructure.Persistence.Repositories;
using EChamado.Server.Infrastructure.Persistence.Repositories.Orders;
using EChamado.Server.Infrastructure.Redis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EChamado.Server.Infrastructure.Configuration;

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

        services.AddScoped<IRedisService, RedisService>();
        services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();

        return services;
    }
}
