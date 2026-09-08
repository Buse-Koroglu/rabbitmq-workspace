using System.Text;
using RabbitMQ.Client;
using RabbitMQ.DirectExchangePublisher;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(
    "logs-direct",
    durable: true,
    type: ExchangeType.Direct
);

foreach (var s in Enum.GetNames(typeof(LogLevel)).ToList())
{
    var queueName = $"direct-queue-{s}";
    var routeKey = $"logs-direct-route-{s}";
    await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
    await channel.QueueBindAsync(queue: queueName, "logs-direct", routeKey,null);
}

var properties = new BasicProperties
{
    Persistent = true
};

foreach (var x in Enumerable.Range(1, 50))
{
    LogLevel log = (LogLevel)Random.Shared.Next(1, 5);
    string message = $"Log Type:  {log}";
    var body = Encoding.UTF8.GetBytes(message);
    var routeKey = $"logs-direct-route-{log}";

    await channel.BasicPublishAsync(
        exchange: "logs-direct",
        routingKey: routeKey,
        mandatory: true,
        basicProperties: properties,
        body: body
    );

    Console.WriteLine($"Log gönderildi: {message}");
}

Console.ReadLine();