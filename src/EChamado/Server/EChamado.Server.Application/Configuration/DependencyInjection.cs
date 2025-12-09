using EChamado.Server.Application.Common;
using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.Services;
using EChamado.Server.Application.Services.AI;
using EChamado.Server.Application.Services.AI.Configuration;
using EChamado.Server.Application.Services.AI.Interfaces;
using EChamado.Server.Application.Services.AI.Providers;
using EChamado.Server.Domain.Services.Interface;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
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

        var currentAssembly = Assembly.GetAssembly(typeof(DependencyInjection));
        if (currentAssembly != null)
        {
            // Configure Paramore.Brighter with explicit assembly scanning
            services.AddBrighter(options =>
            {
                options.HandlerLifetime = ServiceLifetime.Scoped;
            })
            .AutoFromAssemblies(new[] { currentAssembly });

            // Add MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(currentAssembly));

            // Add FluentValidation validators
            services.AddValidatorsFromAssembly(currentAssembly);

            // Add validation behavior to MediatR pipeline
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
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

    /// <summary>
    /// Add AI services for Natural Language to Gridify query conversion
    /// </summary>
    public static IServiceCollection AddAIServices(
        this IServiceCollection services,
        Action<AISettings>? configureSettings = null)
    {
        // Configure settings
        if (configureSettings != null)
        {
            services.Configure(configureSettings);
        }

        // Add memory cache for AI response caching
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // Limit cache size
        });

        // Register AI providers
        services.AddSingleton<IAIProvider, OpenAIProvider>();
        services.AddSingleton<IAIProvider, GeminiProvider>();
        services.AddSingleton<IAIProvider, OpenRouterProvider>();

        // Register provider factory
        services.AddSingleton<AIProviderFactory>();

        // Register main NL to Gridify service
        services.AddScoped<NLToGridifyService>();

        // Add HttpClient for OpenRouter
        services.AddHttpClient("OpenRouter")
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

        return services;
    }
}