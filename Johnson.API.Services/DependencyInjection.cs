using Johnson.API.Services.Discovery;
using Johnson.API.Services.Gateway;
using Johnson.API.Services.InternalSecurity;
using Johnson.Infra.DataStorage.NonGeneric;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Johnson.API.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IRegistryService, RegistryService>();
        services.AddScoped<IProxyService, ProxyService>();

        services.AddSingleton(sp =>
        {
            var secretKey = config["APIKeySecret"] ?? throw new Exception("Cannot find API Secret Key");
            var repository = sp.GetRequiredService<IAPIKeyRepository>();

            return new APIKeyValidatorService(repository, secretKey);
        });

        return services;
    }
}
