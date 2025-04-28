using Johnson.Common.Models.Registry;
using Johnson.ServiceAdapter.InternalSecurity;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Johnson.ServiceAdapter.Client;

public class DiscoveryClient : IDiscoveryClient
{
    private readonly string _proxyBaseURL;
    private readonly string _proxyAPIVersion;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAPIKeyGenerator _apiKeyGenerator;

    public DiscoveryClient(
        IHttpClientFactory httpClientFactory,
        IAPIKeyGenerator apiKeyGenerator,
        string proxyBaseURL,
        string proxyAPIVersion)
    {
        _httpClientFactory = httpClientFactory;
        _apiKeyGenerator = apiKeyGenerator;
        _proxyBaseURL = proxyBaseURL;
       _proxyAPIVersion = proxyAPIVersion;
    }

    public async Task<IList<ServiceListResponse>> GetServicesListAsync()
    {
        var httpClient = _httpClientFactory.CreateClient();
        var apiKey = _apiKeyGenerator.GenerateKey();

        try
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_proxyBaseURL + "api/" + _proxyAPIVersion + "/discovery/get-services"),
            };
            request.Headers.Add("johnson-api-key", apiKey);

            var response = await httpClient.SendAsync(request);
            var rawServicesList = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IList<ServiceListResponse>>(rawServicesList)
                ?? throw new Exception("Failed to deserialize service list.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return [];
        }
    }

    public Task<HttpResponse> SendRequestToServiceAsync()
    {
        throw new NotImplementedException();
    }
}
