using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.Services;
using EChamado.Server.Domain.Services.Interface;
using FluentValidation;
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

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Configure Paramore.Brighter
        services.AddBrighter(options =>
        {
            // Register all handlers from this assembly
            options.HandlerLifetime = ServiceLifetime.Scoped;
        })
        .AutoFromAssemblies(typeof(DependencyInjection).Assembly);

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
        services.AddScoped<IUserTokenService, UserTokenService>();
        services.AddScoped<IOpenIddictService, OpenIddictService>();

        return services;
    }
}

