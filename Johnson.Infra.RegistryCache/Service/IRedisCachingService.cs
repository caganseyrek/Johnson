using Johnson.Common.Models.DataStorage;

namespace Johnson.Infra.RegistryCache.Service;

public interface IRedisCachingService
{
    Task<RegistryServiceEntry?> GetServiceEntryAsync(Guid id);
    Task SetServiceEntryAsync(RegistryServiceEntry serviceEntry, double? ttl = null);
    Task RemoveServiceEntryAsync(Guid id);
    Task ClearCache();
}
