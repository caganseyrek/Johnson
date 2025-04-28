using Johnson.Common.Models.Base;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Johnson.Infra.EventBus.RabbitMQ;

public class RabbitMQPublisher
{
    private readonly IChannel _channel;

    public RabbitMQPublisher(RabbitMQConnectionFactory factory)
    {
        _channel = factory.CreateChannel().Result;
        _channel.ExchangeDeclareAsync("event_queue", ExchangeType.Fanout, durable: true).Wait();
    }

    public async Task PublishEventAsync<T>(T eventObject) where T : EventBase
    {
        string messageKey = $"{nameof(eventObject)}";
        string messageContent = JsonSerializer.Serialize(eventObject, eventObject.GetType());
        string message = $"{messageKey}:${messageContent}";

        byte[] body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync("event_queue", "", body);
    }
}
