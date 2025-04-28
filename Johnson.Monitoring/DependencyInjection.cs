using Johnson.Monitoring.Event;
using Microsoft.Extensions.DependencyInjection;

namespace Johnson.Monitoring;

public static class DependencyInjection
{
    public static IServiceCollection AddMonitoringTools(this IServiceCollection services)
    {
        services.AddScoped(typeof(IEventLogger<,>), typeof(EventLogger<,>));
        return services;
    }
}
