using Johnson.Infra.DataStorage.NonGeneric;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace Johnson.API.Services.InternalSecurity;

public class APIKeyValidatorService : IAPIKeyValidatorService
{
    private readonly IAPIKeyRepository _usedAPIKeyRepository;
    private readonly string _secretKey;

    public APIKeyValidatorService(IAPIKeyRepository usedAPIKeyRepository, string secretKey)
    {
        _usedAPIKeyRepository = usedAPIKeyRepository;
        _secretKey = secretKey;
    }

    public async Task<bool> Validate(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("johnson-api-key", out var value))
            return false;

        string? apiKey = value.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
            return false;

        string[] keySections = apiKey.Split(":".ToCharArray());
        if (keySections.Length != 3)
            return false;

        string randomKey = keySections[0];
        if (!long.TryParse(keySections[1], out var timestamp))
            return false;

        string providedSignature = keySections[2];

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now - timestamp > TimeSpan.FromMinutes(2.5).TotalMilliseconds)
            return false;

        try
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            string dataToSignature = $"{randomKey}:{timestamp}";

            var expectedSignatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSignature));
            var providedSignatureBytes = Convert.FromBase64String(providedSignature);

            if (!CryptographicOperations.FixedTimeEquals(expectedSignatureBytes, providedSignatureBytes))
                return false;
        }
        catch
        {
            return false;
        }
        
        if (await _usedAPIKeyRepository.DoesKeyExists(apiKey))
            return false;

        await _usedAPIKeyRepository.SaveUsedKey(apiKey);
        return true;
    }
}
