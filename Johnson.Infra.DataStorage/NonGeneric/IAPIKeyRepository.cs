namespace Johnson.Infra.DataStorage.NonGeneric;

public interface IAPIKeyRepository
{
    public Task<bool> DoesKeyExists(string key);
    public Task SaveUsedKey(string key);
}
