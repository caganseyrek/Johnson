using Johnson.Infra.DataStorage.NonGeneric;
using Johnson.ServiceAdapter.Client;
using Johnson.ServiceAdapter.InternalSecurity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Johnson.ServiceAdapter;

public static class DependencyInjection
{
    public static IServiceCollection AddJohnsonServiceAdapter(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IAPIKeyGenerator>(sp =>
        {
            var repository = sp.GetRequiredService<IAPIKeyRepository>();
            var secretKey = config["APIKeySecret"] ?? throw new Exception("Cannot find API Secret Key");

            return new APIKeyGenerator(secretKey);
        });

        services.AddSingleton(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var apiKeyGenerator = sp.GetRequiredService<IAPIKeyGenerator>();
            var proxyBaseUrl = config["ProxyBaseURL"] ?? throw new Exception("Cannot find Proxy Base URL");
            var proxyAPIVersion = config["ProxyAPIVersion"] ?? throw new Exception("Cannot find Proxy API Version");

            return new DiscoveryClient(httpClientFactory, apiKeyGenerator, proxyBaseUrl, proxyAPIVersion);
        });

        return services;
    }
}
