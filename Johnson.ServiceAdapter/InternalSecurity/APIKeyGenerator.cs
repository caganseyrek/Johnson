using System.Security.Cryptography;
using System.Text;

namespace Johnson.ServiceAdapter.InternalSecurity;

public class APIKeyGenerator : IAPIKeyGenerator
{
    private readonly string _secretKey;

    public APIKeyGenerator(string secretKey)
    {
        _secretKey = secretKey;
    }

    public string GenerateKey()
    {
        byte[] keyBytes = RandomNumberGenerator.GetBytes(32);

        string randomKey = Convert.ToBase64String(keyBytes);
        string rawData = $"{randomKey}:{DateTime.UtcNow.Ticks}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        string signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData)));
        string apiKey = $"{rawData}:{signature}";

        return apiKey;
    }
}
