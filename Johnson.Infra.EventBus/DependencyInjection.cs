using Johnson.Infra.EventBus.Abstractions;
using Johnson.Infra.EventBus.EventHandlers;
using Johnson.Infra.EventBus.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Johnson.Infra.EventBus;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration config)
    {
        var rabbitMQOptions = config.GetSection("RabbitMQ").Get<RabbitMQOptions>()
            ?? throw new Exception("Cannot find RabbitMQ Config Section");

        rabbitMQOptions.Validate();

        services.AddSingleton(new RabbitMQConnectionFactory(rabbitMQOptions));
        services.AddSingleton<RabbitMQPublisher>();
        services.AddHostedService<RabbitMQConsumer>();

        services.AddScoped<IEventHandler, DbChangeEventHandler>();
        services.AddScoped<IEventHandler, TTLUpdateEventHandler>();

        return services;
    }
}
