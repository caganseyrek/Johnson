using RabbitMQ.Client;

namespace Johnson.Infra.EventBus.RabbitMQ;

public class RabbitMQConnectionFactory
{
    private readonly IConnection _connection;

    public RabbitMQConnectionFactory(RabbitMQOptions options)
    {
        var connectionFactory = new ConnectionFactory()
        {
            HostName = options.HostName,
            Port = int.Parse(options.Port),
            UserName = options.UserName,
            Password = options.Password,
        };
        _connection = connectionFactory.CreateConnectionAsync().Result;
    }
    public async Task<IChannel> CreateChannel() => await _connection.CreateChannelAsync();
}

public class RabbitMQOptions
{
    public required string HostName { get; set; }
    public required string Port { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }

    public void Validate()
    {
        var missingProps = GetType().GetProperties()
                             .Where(p => p.PropertyType == typeof(string))
                             .Where(p => string.IsNullOrWhiteSpace((string?)p.GetValue(this)))
                             .Select(p => p.Name).ToList();

        if (missingProps.Count > 0)
        {
            string exceptionMessage =
                $"Following fields are missing from RabbitMQ Config Section: {string.Join(", ", missingProps)}";

            throw new Exception(exceptionMessage);
        }
    }
}