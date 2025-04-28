using Johnson.Infra.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Johnson.Infra.EventBus.RabbitMQ;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(
        RabbitMQConnectionFactory factory,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _channel = factory.CreateChannel().Result;
        _channel.ExchangeDeclareAsync("event_queue", ExchangeType.Fanout, durable: true).Wait();
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        var queueName = _channel.QueueDeclareAsync(cancellationToken: ct).Result.QueueName;
        _channel.QueueBindAsync(queue: queueName, exchange: "event_queue", routingKey: "", cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var separatorIndex = message.IndexOf(":");
            var key = message[..separatorIndex];
            var value = message[(separatorIndex + 1)..];

            using var scope = _serviceProvider.CreateAsyncScope();
            var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IEventHandler>>();
            foreach (var handler in handlers.Where(h => h.EventKey == key))
            {
                await handler.HandleAsync(value);
            }
        };
        _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer, cancellationToken: ct);

        return Task.CompletedTask;
    }
}