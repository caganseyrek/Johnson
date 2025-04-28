using Johnson.Common.Models.Event;
using Johnson.Infra.EventBus.Abstractions;
using Johnson.Monitoring.Event;
using System.Text.Json;

namespace Johnson.Infra.EventBus.EventHandlers;

public class FailureEventHandler : IEventHandler
{
    private readonly IEventLogger<FailureEvent, FailureEventHandler> _eventLogger;

    public FailureEventHandler(IEventLogger<FailureEvent, FailureEventHandler> eventLogger)
    {
        _eventLogger = eventLogger;
    }

    public string EventKey => nameof(FailureEventHandler);

    public async Task HandleAsync(string message)
    {
        FailureEvent? eventDetails = null;

        try
        {
            eventDetails = JsonSerializer.Deserialize<FailureEvent>(message);
            if (eventDetails is null)
            {
                await _eventLogger.LogEventFailureAsync(
                    eventId: eventDetails?.Id,
                    errorMessage: "Failed to deserialize the event",
                    eventPublishedAt: eventDetails?.EventPublishedAt,
                    isCritical: true);

                return;
            }

            await _eventLogger.LogEventFailureAsync(
                eventId: eventDetails?.Id,
                errorMessage: $"An failure occurred: {eventDetails?.FailureMessage} (Type: {eventDetails?.FailureType})",
                eventPublishedAt: eventDetails?.EventPublishedAt,
                isCritical: true);
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
}
