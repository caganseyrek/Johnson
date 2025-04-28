using Johnson.Infra.RegistryCache.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Johnson.Infra.RegistryCache;

public static class DependencyInjection
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration config)
    {
        var redisConnectionString = config.GetConnectionString("Redis")
            ?? throw new Exception("Cannot find Redis Connection String");

        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = redisConnectionString;
            redisOptions.InstanceName = "johnson_cache";
            redisOptions.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectRetry = 5,
            };
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var connectionMultiplexerOptions = new ConfigurationOptions
            {
                EndPoints = { redisConnectionString },
                AbortOnConnectFail = false,
                ConnectRetry = 5,
            };

            var connection = ConnectionMultiplexer.ConnectAsync(connectionMultiplexerOptions).Result;
            return connection;
        });

        services.AddSingleton<IRedisCachingService, RedisCachingService>();

        return services;
    }
}
