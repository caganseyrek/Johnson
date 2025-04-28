using Johnson.Common;
using Johnson.Common.Models.DataStorage;
using Johnson.Common.Models.Event;
using Johnson.Infra.DataStorage.Generic;
using Johnson.Infra.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Http;

namespace Johnson.API.Services.Gateway;

public class ProxyService : IProxyService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRepository<RegistryServiceEntry> _registryServiceEntryRepository;
    private readonly RabbitMQPublisher _rabbitMQPublisher;

    public ProxyService(
        IHttpClientFactory httpClientFactory,
        IRepository<RegistryServiceEntry> registryServiceEntryRepository,
        RabbitMQPublisher rabbitMQPublisher)
    {
        _httpClientFactory = httpClientFactory;
        _registryServiceEntryRepository = registryServiceEntryRepository;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    public async Task<HttpResponseMessage?> ForwardRequestAsync(HttpContext httpContext)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var request = httpContext.Request;

        try
        {
            var destinationServiceURL = await GetDestinationServiceURL(request);
            
            var bodyStream = new MemoryStream();
            request.Body.Position = 0;
            await request.Body.CopyToAsync(bodyStream);
            bodyStream.Position = 0;

            var content = new StreamContent(bodyStream);
            foreach (var header in request.Headers)
            {
                if (!content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            var forwardedRequest = new HttpRequestMessage
            {
                Method = new HttpMethod(request.Method),
                RequestUri = new Uri(destinationServiceURL),
                Content = content,
            };
            var response = await httpClient.SendAsync(forwardedRequest);

            var requestForwardedEvent = new RequestForwardedEvent
            {
                Id = Guid.NewGuid(),
                EventPublishedAt = DateTime.UtcNow,
                OriginService = nameof(ForwardRequestAsync),
            };
            await _rabbitMQPublisher.PublishEventAsync(requestForwardedEvent);
            return response;
        }
        catch (Exception ex)
        {
            var failureEvent = new FailureEvent
            {
                Id = Guid.NewGuid(),
                OriginService = nameof(ForwardRequestAsync),
                EventPublishedAt = DateTime.UtcNow,
                FailureType = FailureType.CriticalError,
                FailureMessage = ex.Message,
            };
            await _rabbitMQPublisher.PublishEventAsync(failureEvent);
            return null;
        }
    }

    private async Task<string> GetDestinationServiceURL(HttpRequest httpRequest)
    {
        var serviceName = httpRequest.Headers.Where(x => x.Key.Equals("johnson-forward-to")).FirstOrDefault().Value.ToString();
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            throw new Exception($"Invalid service name: {serviceName}");
        }

        var existingService = await _registryServiceEntryRepository.GetEntitiesByAsync(x => x.Name.Equals(serviceName));
        if (existingService.Count != 1)
        {
            throw new Exception($"Failed to find an existing service with name: {serviceName}");
        }

        return existingService.ElementAt(0).URL;
    }
}
