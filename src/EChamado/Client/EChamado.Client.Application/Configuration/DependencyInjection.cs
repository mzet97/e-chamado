using EChamado.Client.Application.UseCases.Auth.Handlers;
using EChamado.Client.Application.UseCases.Auth.Handlers.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EChamado.Client.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection ResolveDependenciesApplication(this IServiceCollection services)
    {
        services.AddScoped<ILoginHandler, LoginHandler>();
        services.AddScoped<IRegisterHandler, RegisterHandler>();

        return services;
    }
}
