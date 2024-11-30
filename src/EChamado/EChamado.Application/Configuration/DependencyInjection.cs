using EChamado.Application.Common.Behaviours;
using EChamado.Application.ViewModels;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EChamado.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddValidatorsFromAssembly(typeof(BaseSearch).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(BaseSearch).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        return services;
    }
}

