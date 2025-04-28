namespace Johnson.Monitoring.Event;

public interface IEventLogger<TEvent, TLogger>
{
    Task LogEventSuccessAsync(Guid? eventId, DateTime eventPublishTimestamp);
    Task LogEventFailureAsync(
        Guid? eventId,
        DateTime? eventPublishedAt,
        string errorMessage,
        bool isCritical = false);
}
