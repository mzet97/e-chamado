﻿using EChamado.Server.Application.Configuration;
using EChamado.Server.Infrastructure.Configuration;

namespace EChamado.Server.Configuration;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddIdentityConfig(configuration);
        services.AddCorsConfig();
        services.ResolveDependenciesInfrastructure();
        services.ResolveDependenciesApplication();

        services.AddMessageBus(configuration);
        services.AddApplicationServices();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerConfig();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        services.AddAuthorization();
        services.AddApiResponseCompression();

        services.AddControllers();

        return services;
    }
}
