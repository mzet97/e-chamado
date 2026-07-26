using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.Services;
using EChamado.Server.Application.Services.AI;
using EChamado.Server.Application.Services.AI.Configuration;
using EChamado.Server.Application.Services.AI.Interfaces;
using EChamado.Server.Application.Services.AI.Providers;
using EChamado.Server.Application.UseCases.Categories.Queries.Handlers;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Application.UseCases.Comments.Queries.Handlers;
using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Server.Application.UseCases.Departments.Queries;
using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using EChamado.Server.Application.UseCases.OrderTypes.Queries.Handlers;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Server.Application.UseCases.StatusTypes.Queries.Handlers;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Application.UseCases.SubCategories.Queries.Handlers;
using EChamado.Server.Application.UseCases.SubCategories.ViewModels;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Darker;
using System.Reflection;
using CategoriesQueries = EChamado.Server.Application.UseCases.Categories.Queries;
using OrdersQueries = EChamado.Server.Application.UseCases.Orders.Queries;
using CommentsQueries = EChamado.Server.Application.UseCases.Comments.Queries;
using OrderTypesQueries = EChamado.Server.Application.UseCases.OrderTypes.Queries;
using StatusTypesQueries = EChamado.Server.Application.UseCases.StatusTypes.Queries;
using SubCategoriesQueries = EChamado.Server.Application.UseCases.SubCategories.Queries;
using OrdersQueryHandlers = EChamado.Server.Application.UseCases.Orders.Queries.Handlers;

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

            // Register Darker query processor (custom ServiceProviderQueryProcessor resolves via DI)
            services.AddScoped<IQueryProcessor, ServiceProviderQueryProcessor>();

            // Register Gridify query handlers (Darker). Handlers de commands (Brighter)
            // são resolvidos automaticamente pelo AutoFromAssemblies acima.
            services.AddScoped<IQueryHandler<CategoriesQueries.GridifyCategoryQuery, BaseResultList<CategoryViewModel>>, GridifyCategoryQueryHandler>();
            services.AddScoped<IQueryHandler<OrdersQueries.GridifyOrderQuery, BaseResultList<OrderViewModel>>, OrdersQueryHandlers.GridifyOrderQueryHandler>();
            services.AddScoped<IQueryHandler<CommentsQueries.GridifyCommentQuery, BaseResultList<CommentViewModel>>, GridifyCommentQueryHandler>();
            services.AddScoped<IQueryHandler<GridifyDepartmentQuery, BaseResultList<DepartmentViewModel>>, GridifyDepartmentQueryHandler>();
            services.AddScoped<IQueryHandler<OrderTypesQueries.GridifyOrderTypeQuery, BaseResultList<OrderTypeViewModel>>, GridifyOrderTypeQueryHandler>();
            services.AddScoped<IQueryHandler<StatusTypesQueries.GridifyStatusTypeQuery, BaseResultList<StatusTypeViewModel>>, GridifyStatusTypeQueryHandler>();
            services.AddScoped<IQueryHandler<SubCategoriesQueries.GridifySubCategoryQuery, BaseResultList<SubCategoryViewModel>>, GridifySubCategoryQueryHandler>();

            // Add FluentValidation validators
            services.AddValidatorsFromAssembly(currentAssembly);
        }

        // Register the generic validation and exception handlers for Brighter pipeline
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
        if (configureSettings != null)
        {
            services.Configure(configureSettings);
        }

        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024;
        });

        services.AddSingleton<IAIProvider, OpenAIProvider>();
        services.AddSingleton<IAIProvider, GeminiProvider>();
        services.AddSingleton<IAIProvider, OpenRouterProvider>();

        services.AddSingleton<AIProviderFactory>();
        services.AddScoped<NLToGridifyService>();

        services.AddHttpClient("OpenRouter")
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // Em dev, aceita certificados self-signed/unknown para chamadas HTTPS externas
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });

        return services;
    }
}
