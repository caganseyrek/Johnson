using Johnson.Common.Models.DataStorage;

namespace Johnson.Infra.DataStorage.NonGeneric;

public class APIKeyRepository : IAPIKeyRepository
{
    private readonly JohnsonDbContext _context;

    public APIKeyRepository(JohnsonDbContext context)
    {
        _context = context;
    }

    public async Task<bool> DoesKeyExists(string key)
    {
        return await _context.InvalidatedAPIKey.FindAsync(key) != null;
    }

    public async Task SaveUsedKey(string key)
    {
        InvalidatedAPIKey usedAPIKey = new() { Key = key };
        await _context.InvalidatedAPIKey.AddAsync(usedAPIKey);
        await _context.SaveChangesAsync();
    }
}
