using Johnson.Infra.DataStorage.Generic;
using Johnson.Infra.DataStorage.NonGeneric;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Johnson.Infra.DataStorage;

public static class DependencyInjection
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<JohnsonDbContext>(options =>
            options.UseSqlite(config.GetConnectionString("SQLite")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAPIKeyRepository, APIKeyRepository>();

        return services;
    }
}
