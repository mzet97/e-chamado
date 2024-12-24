using EChamado.Infrastructure.Redis;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EChamado.Infrastructure.Configuration;

public static class RedisConfigExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection("Redis:ConnectionString").Value;
        var redisInstanceName = configuration.GetSection("Redis:InstanceName").Value;

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConfiguration));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfiguration;
            options.InstanceName = redisInstanceName;
        });

        return services;
    }

    public static IServiceCollection AddRedisOutputCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection("Redis:ConnectionString").Value;

        services.AddSingleton<IOutputCacheStore, RedisOutputCacheStore>(sp =>
        {
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            return new RedisOutputCacheStore(multiplexer.GetDatabase());
        });

        services.AddOutputCache(options =>
        {
            options.AddPolicy("DefaultPolicy", builder =>
            {
                builder.Expire(TimeSpan.FromMinutes(5));
            });
        });

        return services;
    }
}