using Johnson.Common.Models.DataStorage;
using Johnson.Common.Models.Event;
using Johnson.Infra.DataStorage.Generic;
using Johnson.Infra.EventBus.Abstractions;
using Johnson.Infra.RegistryCache.Service;
using Johnson.Monitoring.Event;
using System.Text.Json;

namespace Johnson.Infra.EventBus.EventHandlers;

public class TTLUpdateEventHandler : IEventHandler
{
    private readonly IRedisCachingService _redisCachingService;
    private readonly IRepository<RegistryServiceEntry> _registryServiceEntryRepository;
    private readonly IEventLogger<TTLUpdateEvent, TTLUpdateEventHandler> _eventLogger;

    public TTLUpdateEventHandler(
        IRedisCachingService redisCachingService,
        IRepository<RegistryServiceEntry> registryServiceEntryRepository,
        IEventLogger<TTLUpdateEvent, TTLUpdateEventHandler> eventLogger)
    {
        _redisCachingService = redisCachingService;
        _registryServiceEntryRepository = registryServiceEntryRepository;
        _eventLogger = eventLogger;
    }

    public string EventKey => nameof(TTLUpdateEvent);

    public async Task HandleAsync(string message)
    {
        TTLUpdateEvent? eventDetails = null;

        try
        {
            eventDetails = JsonSerializer.Deserialize<TTLUpdateEvent>(message);
            if (eventDetails is null)
            {
                await _eventLogger.LogEventFailureAsync(
                    eventId: eventDetails?.Id,
                    errorMessage: "Failed to deserialize the event",
                    eventPublishedAt: eventDetails?.EventPublishedAt,
                    isCritical: true);

                return;
            }

            var updatedEntryDetails = await _registryServiceEntryRepository.GetEntityAsync(eventDetails.EffectedDataId);
            if (updatedEntryDetails is null)
            {
                await _eventLogger.LogEventFailureAsync(
                    eventId: eventDetails?.Id,
                    errorMessage: $"Failed to retrieve the updated registry service entry details with ID: {eventDetails?.EffectedDataId}",
                    eventPublishedAt: eventDetails?.EventPublishedAt,
                    isCritical: true);

                return;
            }

            await _redisCachingService.SetServiceEntryAsync(updatedEntryDetails);

            await _eventLogger.LogEventSuccessAsync(
                eventId: eventDetails?.Id,
                eventPublishTimestamp: eventDetails?.EventPublishedAt ?? DateTime.UnixEpoch);
        }
        catch (Exception)
        {
            await _eventLogger.LogEventFailureAsync(
                eventId: eventDetails?.Id,
                errorMessage: "Failed to deserialize the event",
                eventPublishedAt: eventDetails?.EventPublishedAt,
                isCritical: true);

            return;
        }
    }
}
