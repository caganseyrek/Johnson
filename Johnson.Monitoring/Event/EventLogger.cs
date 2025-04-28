using Johnson.Common;
using Johnson.Common.Models.DataStorage;
using Johnson.Infra.DataStorage.Generic;
using Microsoft.Extensions.Logging;

namespace Johnson.Monitoring.Event;

public class EventLogger<TEvent, TLogger> : IEventLogger<TEvent, TLogger> where TEvent : class where TLogger : class
{
    private readonly string _eventName;
    private readonly string _loggerName;

    private readonly ILogger<EventLogger<TEvent, TLogger>> _logger;
    private readonly IRepository<EventAuditLog> _eventLogRepository;

    public EventLogger(ILogger<EventLogger<TEvent, TLogger>> logger, IRepository<EventAuditLog> eventLogRepository)
    {
        _eventName = nameof(TEvent);
        _loggerName = nameof(TLogger);

        _logger = logger;
        _eventLogRepository = eventLogRepository;
    }

    public async Task LogEventSuccessAsync(Guid? eventId, DateTime eventPublishTimestamp)
    {
        _logger.LogInformation("New successful {eventName} logged", _eventName);
        var eventLog = new EventAuditLog
        {
            Id = eventId ?? Guid.NewGuid(),
            EventName = _eventName,
            OriginService = _loggerName,
            EventPublishedAt = eventPublishTimestamp,
            Outcome = EventOutcome.Success,
            ErrorMessage = null,
            LoggedAt = DateTime.UtcNow,
        };
        await _eventLogRepository.CreateNewEntityAsync(eventLog);
    }

    public async Task LogEventFailureAsync(
        Guid? eventId,
        DateTime? eventPublishedAt,
        string errorMessage,
        bool isCritical = false)
    {
        if (isCritical)
            _logger.LogCritical("{eventName} failed critically", _eventName);
        else
            _logger.LogError("{eventName} failed", _eventName);

        var eventLog = new EventAuditLog
        {
            Id = eventId ?? Guid.NewGuid(),
            EventName = _eventName,
            OriginService = _loggerName,
            Outcome = isCritical ? EventOutcome.CriticalFailure : EventOutcome.Failure,
            ErrorMessage = errorMessage,
            EventPublishedAt = eventPublishedAt,
            LoggedAt = DateTime.UtcNow,
        };
        await _eventLogRepository.CreateNewEntityAsync(eventLog);
    }
}
