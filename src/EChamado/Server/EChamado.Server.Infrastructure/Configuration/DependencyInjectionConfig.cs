using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Repositories;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Server.Application.Orders.Events;
using EChamado.Server.Domain.Domains.Orders.Events.Categories;
using EChamado.Server.Domain.Domains.Orders.Events.Comments;
using EChamado.Server.Domain.Domains.Orders.Events.Departments;
using EChamado.Server.Domain.Domains.Orders.Events.OrderTypes;
using EChamado.Server.Domain.Domains.Orders.Events.Orders;
using EChamado.Server.Domain.Domains.Orders.Events.StatusTypes;
using EChamado.Server.Domain.Domains.Orders.Events.SubCategories;
using EChamado.Server.Infrastructure.Email;
using EChamado.Server.Infrastructure.Events;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Server.Infrastructure.Persistence.Repositories;
using EChamado.Server.Infrastructure.Persistence.Repositories.Orders;
using EChamado.Server.Infrastructure.Redis;
using EChamado.Server.Infrastructure.Services;
using EChamado.Shared.Services;
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
        services.AddScoped<IUserTokenService, UserTokenService>();

        services.AddScoped<IRedisService, RedisService>();
        services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();

        services.AddSingleton<IBrighterEventMapper>(sp => new BrighterEventMapper()
            .Register<OrderCreated>(e => new OrderCreatedBrighterEvent(e))
            .Register<OrderUpdated>(e => new OrderUpdatedBrighterEvent(e))
            .Register<OrderClosed>(e => new OrderClosedBrighterEvent(e))
            .Register<CategoryCreated>(e => new CategoryCreatedBrighterEvent(e))
            .Register<CategoryUpdated>(e => new CategoryUpdatedBrighterEvent(e))
            .Register<DepartmentCreated>(e => new DepartmentCreatedBrighterEvent(e))
            .Register<DepartmentUpdated>(e => new DepartmentUpdatedBrighterEvent(e))
            .Register<OrderTypeCreated>(e => new OrderTypeCreatedBrighterEvent(e))
            .Register<OrderTypeUpdated>(e => new OrderTypeUpdatedBrighterEvent(e))
            .Register<StatusTypeCreated>(e => new StatusTypeCreatedBrighterEvent(e))
            .Register<StatusTypeUpdated>(e => new StatusTypeUpdatedBrighterEvent(e))
            .Register<SubCategoryCreated>(e => new SubCategoryCreatedBrighterEvent(e))
            .Register<SubCategoryUpdated>(e => new SubCategoryUpdatedBrighterEvent(e))
            .Register<CommentCreated>(e => new CommentCreatedBrighterEvent(e))
            .Register<CommentDeleted>(e => new CommentDeletedBrighterEvent(e)));

        services.AddScoped<IDomainEventDispatcher, BrighterDomainEventDispatcher>();
        services.AddScoped<DomainEventsSaveChangesInterceptor>();

        // Date/Time provider for testable timestamps
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
