using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.Services;
using EChamado.Server.Domain.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using System.Reflection;

namespace EChamado.Server.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient();

        // Configure Paramore.Brighter with explicit assembly scanning
        var currentAssembly = Assembly.GetAssembly(typeof(DependencyInjection));
        if (currentAssembly != null)
        {
            services.AddBrighter(options =>
            {
                options.HandlerLifetime = ServiceLifetime.Scoped;
            })
            .AutoFromAssemblies(new[] { currentAssembly });
        }

        // Register the generic validation and exception handlers
        services.AddTransient(typeof(ValidationHandler<>));
        services.AddTransient(typeof(UnhandledExceptionHandler<>));

        return services;
    }

    public static IServiceCollection ResolveDependenciesApplication(this IServiceCollection services)
    {
        services.AddScoped<IApplicationUserService, ApplicationUserService>();
        services.AddScoped<IRoleClaimService, RoleClaimService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserClaimService, UserClaimService>();
        services.AddScoped<IUserLoginService, UserLoginService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IOpenIddictService, OpenIddictService>();

        return services;
    }
}