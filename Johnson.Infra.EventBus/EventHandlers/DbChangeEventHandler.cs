using Johnson.Common;
using Johnson.Common.Models.DataStorage;
using Johnson.Common.Models.Event;
using Johnson.Infra.DataStorage.Generic;
using Johnson.Infra.EventBus.Abstractions;
using Johnson.Infra.RegistryCache.Service;
using Johnson.Monitoring.Event;
using System.Text.Json;

namespace Johnson.Infra.EventBus.EventHandlers;

public class DbChangeEventHandler : IEventHandler
{
    private readonly IRedisCachingService _redisCachingService;
    private readonly IRepository<RegistryServiceEntry> _registryServiceEntryrepository;
    private readonly IEventLogger<DbChangeEvent, DbChangeEventHandler> _eventLogger;

    public DbChangeEventHandler(
        IRedisCachingService redisCachingService,
        IRepository<RegistryServiceEntry> registryServiceEntryrepository,
        IEventLogger<DbChangeEvent, DbChangeEventHandler> eventLogger)
    {
        _registryServiceEntryrepository = registryServiceEntryrepository;
        _redisCachingService = redisCachingService;
        _eventLogger = eventLogger;
    }

    public string EventKey => nameof(DbChangeEvent);

    public async Task HandleAsync(string message)
    {
        DbChangeEvent? eventDetails = null;
        try
        {
            eventDetails = JsonSerializer.Deserialize<DbChangeEvent>(message);
            if (eventDetails is null)
            {
                await _eventLogger.LogEventFailureAsync(
                    eventId: eventDetails?.Id,
                    errorMessage: "Failed to deserialize the event",
                    eventPublishedAt: eventDetails?.EventPublishedAt,
                    isCritical: true);

                return;
            }

            switch (eventDetails.ChangeType)
            {
                case DbChangeType.Create or DbChangeType.Update:
                    await HandleCreateOrUpdateAsync(eventDetails);
                    break;

                case DbChangeType.Delete:
                    await HandleDeleteAsync(eventDetails);
                    break;

                default:
                    await HandleInvalidChangeTypeAsync(eventDetails);
                    break;
            }
        }
        catch (Exception ex)
        {
            await _eventLogger.LogEventFailureAsync(
                eventId: eventDetails?.Id,
                errorMessage: ex.Message,
                eventPublishedAt: eventDetails?.EventPublishedAt,
                isCritical: true);
        }
    }

    private async Task HandleCreateOrUpdateAsync(DbChangeEvent eventDetails)
    {
        var updatedEntry = await _registryServiceEntryrepository.GetEntityAsync(eventDetails.EffectedDataId);
        if (updatedEntry is null)
        {
            await _eventLogger.LogEventFailureAsync(
                eventId: eventDetails?.Id,
                errorMessage: $"Failed to save registry service entry with ID: {eventDetails?.EffectedDataId}",
                eventPublishedAt: eventDetails?.EventPublishedAt,
                isCritical: true);

            return;
        }
        await _redisCachingService.SetServiceEntryAsync(updatedEntry);

        await _eventLogger.LogEventSuccessAsync(
            eventId: eventDetails.Id,
            eventPublishTimestamp: eventDetails.EventPublishedAt ?? DateTime.UnixEpoch);
    }

    private async Task HandleDeleteAsync(DbChangeEvent eventDetails)
    {
        await _redisCachingService.RemoveServiceEntryAsync(eventDetails.EffectedDataId);

        await _eventLogger.LogEventSuccessAsync(
            eventId: eventDetails.Id,
            eventPublishTimestamp: eventDetails.EventPublishedAt ?? DateTime.UnixEpoch);
    }

    private async Task HandleInvalidChangeTypeAsync(DbChangeEvent eventDetails)
    {
        await _eventLogger.LogEventFailureAsync(
            eventId: eventDetails?.Id,
            errorMessage: "Switch reached default case",
            eventPublishedAt: eventDetails?.EventPublishedAt,
            isCritical: true);
    }
}
