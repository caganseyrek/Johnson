using Johnson.API.Services.Utilities;
using Johnson.Common;
using Johnson.Common.Models.DataStorage;
using Johnson.Common.Models.Event;
using Johnson.Common.Models.Registry;
using Johnson.Infra.DataStorage.Generic;
using Johnson.Infra.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Http;

namespace Johnson.API.Services.Discovery;

public class RegistryService : IRegistryService
{
    private readonly IRepository<RegistryServiceEntry> _registryServiceEntryRepository;
    private readonly RabbitMQPublisher _rabbitMQPublisher;

    public RegistryService(
        IRepository<RegistryServiceEntry> registryServiceEntryRepository,
        RabbitMQPublisher rabbitMQPublisher)
    {
        _registryServiceEntryRepository = registryServiceEntryRepository;
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    public async Task<bool> RegisterNewServiceAsync(HttpContext httpContext, ServiceStartupRequest requestBody)
    {
        var (ipAddress, port, URL) = HttpContextUtils.Destructure(httpContext);
        if (ipAddress is null || port is null || URL is null)
        {
            var failureEvent = new FailureEvent
            {
                Id = Guid.NewGuid(),
                EventPublishedAt = DateTime.UtcNow,
                OriginService = nameof(RegisterNewServiceAsync),
                FailureType = FailureType.CriticalError,
                FailureMessage = "Cannot find IP address, HTTP request scheme, or host."
            };
            await _rabbitMQPublisher.PublishEventAsync(failureEvent);
            return false;
        }

        var existingEntry = (await _registryServiceEntryRepository.GetEntitiesByAsync(x => x.URL.Equals(URL))).FirstOrDefault();
        var serviceEntry = new RegistryServiceEntry
        {
            Id = existingEntry != null ? existingEntry.Id : Guid.NewGuid(),
            LastSeen = DateTime.UtcNow,
            IsHealthy = true,
            Name = requestBody.Name,
            URL = URL,
            IP = ipAddress.ToString(),
            Port = port.Value,
            APIVersion = requestBody.APIVersion,
        };

        if (existingEntry != null)
            await _registryServiceEntryRepository.UpdateEntityAsync(existingEntry.Id, serviceEntry);
        else
            await _registryServiceEntryRepository.CreateNewEntityAsync(serviceEntry);

        var dbChangeEvent = new DbChangeEvent
        {
            Id = Guid.NewGuid(),
            EventPublishedAt = DateTime.UtcNow,
            OriginService = nameof(RegisterNewServiceAsync),
            EffectedDataId = serviceEntry.Id,
            ChangeType = DbChangeType.Create,
        };
        await _rabbitMQPublisher.PublishEventAsync(dbChangeEvent);
        return true;
    }

    public async Task<bool> RegisterHeartbeatAsync(HttpContext httpContext)
    {
        var (ipAddress, _, _) = HttpContextUtils.Destructure(httpContext);
        if (ipAddress is null)
        {
            var failureEvent = new FailureEvent
            {
                Id = Guid.NewGuid(),
                EventPublishedAt = DateTime.UtcNow,
                OriginService = nameof(RegisterHeartbeatAsync),
                FailureType = FailureType.CriticalError,
                FailureMessage = "Cannot find IP address."
            };
            await _rabbitMQPublisher.PublishEventAsync(failureEvent);
            return false;
        }

        var existingService = await _registryServiceEntryRepository.GetEntitiesByAsync(x => x.IP.Equals(ipAddress.ToString()));
        if (existingService.Count != 1)
        {
            var failureEvent = new FailureEvent
            {
                Id = Guid.NewGuid(),
                EventPublishedAt = DateTime.UtcNow,
                OriginService = nameof(RegisterHeartbeatAsync),
                FailureType = FailureType.CriticalError,
                FailureMessage = "Cannot find IP address."
            };
            await _rabbitMQPublisher.PublishEventAsync(failureEvent);
            return false;
        }

        var serviceDetails = existingService.ElementAt(0);

        serviceDetails.LastSeen = DateTime.UtcNow;
        await _registryServiceEntryRepository.UpdateEntityAsync(serviceDetails.Id, serviceDetails);

        var ttlUpdateEvent = new TTLUpdateEvent
        {
            Id = Guid.NewGuid(),
            EventPublishedAt = DateTime.UtcNow,
            OriginService = nameof(RegisterHeartbeatAsync),
            EffectedDataId = serviceDetails.Id,
        };
        await _rabbitMQPublisher.PublishEventAsync(ttlUpdateEvent);
        return true;
    }

    public async Task<List<ServiceListResponse>> GetRegisteredServicesAsync(HttpContext httpContext)
    {
        var existingServices = await _registryServiceEntryRepository.GetAllEntitiesAsync();
        var servicesList = existingServices.Select(service => new ServiceListResponse
        {
            Name = service.Name,
            APIVersion = service.APIVersion
        }).ToList();

        return servicesList;
    }
}
