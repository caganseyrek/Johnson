namespace Johnson.Infra.EventBus.Abstractions;

public interface IEventHandler
{
    string EventKey { get; }
    Task HandleAsync(string message);
}
