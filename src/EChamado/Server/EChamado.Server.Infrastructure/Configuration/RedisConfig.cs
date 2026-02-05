using EChamado.Server.Infrastructure.Redis;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace EChamado.Server.Infrastructure.Configuration;

public static class RedisConfigExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            var redisConfiguration = configuration.GetSection("Redis:ConnectionString").Value;
            var redisInstanceName = configuration.GetSection("Redis:InstanceName").Value ?? "EChamado_";

            if (string.IsNullOrEmpty(redisConfiguration))
            {
                throw new InvalidOperationException("Redis connection string is not configured");
            }

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var logger = sp.GetService<ILogger<IConnectionMultiplexer>>();
                try
                {
                    var options = ConfigurationOptions.Parse(redisConfiguration);
                    options.ConnectTimeout = 5000;
                    options.SyncTimeout = 5000;
                    options.AbortOnConnectFail = false;

                    return ConnectionMultiplexer.Connect(options);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Failed to connect to Redis: {ConnectionString}", redisConfiguration);
                    throw;
                }
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfiguration;
                options.InstanceName = redisInstanceName;
            });
        }
        catch (Exception)
        {
            // If Redis fails, use in-memory cache as fallback
            services.AddDistributedMemoryCache();
        }

        return services;
    }

    public static IServiceCollection AddRedisOutputCache(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            var redisConfiguration = configuration.GetSection("Redis:ConnectionString").Value;

            if (string.IsNullOrEmpty(redisConfiguration))
            {
                // Fallback to memory cache
                services.AddSingleton<IOutputCacheStore, MemoryOutputCacheStore>();
                services.AddOutputCache(options =>
                {
                    options.AddPolicy("DefaultPolicy", builder =>
                    {
                        builder.Expire(TimeSpan.FromMinutes(5));
                    });
                });
                return services;
            }

            services.AddSingleton<IOutputCacheStore>(sp =>
            {
                try
                {
                    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                    return new RedisOutputCacheStore(multiplexer.GetDatabase());
                }
                catch
                {
                    // Return memory cache implementation
                    var memoryCache = sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
                    return new MemoryOutputCacheStore(memoryCache);
                }
            });

            services.AddOutputCache(options =>
            {
                options.AddPolicy("DefaultPolicy", builder =>
                {
                    builder.Expire(TimeSpan.FromMinutes(5));
                });
            });
        }
        catch
        {
            // Fallback to default memory cache
            services.AddSingleton<IOutputCacheStore, MemoryOutputCacheStore>();
            services.AddOutputCache(options =>
            {
                options.AddPolicy("DefaultPolicy", builder =>
                {
                    builder.Expire(TimeSpan.FromMinutes(5));
                });
            });
        }

        return services;
    }
}