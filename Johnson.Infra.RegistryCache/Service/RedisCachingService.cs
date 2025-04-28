using Johnson.Common.Models.DataStorage;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Johnson.Infra.RegistryCache.Service;

public class RedisCachingService : IRedisCachingService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    private const string CACHE_KEY_PREFIX = "johnson_cache_entry";

    public RedisCachingService(IDistributedCache cache, IConnectionMultiplexer redisConnection)
    {
        _cache = cache;
        _redisConnection = redisConnection;
    }

    private static string FormatNamespace(Guid id) => $"{CACHE_KEY_PREFIX}:{id}";

    public async Task<RegistryServiceEntry?> GetServiceEntryAsync(Guid id)
    {
        var key = FormatNamespace(id);
        var entry = await _cache.GetStringAsync(key);
        if (entry is not null)
        {
            return JsonSerializer.Deserialize<RegistryServiceEntry>(entry, _serializerOptions);
        }
        return null;
    }

    public async Task SetServiceEntryAsync(RegistryServiceEntry serviceEntry, double? ttl = null)
    {
        var entryTTL = ttl ?? 10;

        var key = FormatNamespace(serviceEntry.Id);
        var options = new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(entryTTL)
        };
        await _cache.SetStringAsync(key, JsonSerializer.Serialize(serviceEntry, _serializerOptions), options);
    }

    public async Task RemoveServiceEntryAsync(Guid id)
    {
        var key = FormatNamespace(id);
        await _cache.RemoveAsync(key);
    }

    public async Task ClearCache()
    {
        var redisDB = _redisConnection.GetDatabase();
        await redisDB.ExecuteAsync("FLUSHDB");
    }
}
