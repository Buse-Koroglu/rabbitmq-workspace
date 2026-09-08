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

await channel.ExchangeDeclareAsync("logs-topic",durable: true, type: ExchangeType.Topic );

var properties = new BasicProperties
{
    Persistent = true
};

foreach (var x in Enumerable.Range(1, 50))
{
    LogLevel log1 = (LogLevel)Random.Shared.Next(1, 5);
    LogLevel log2 = (LogLevel)Random.Shared.Next(1, 5);
    LogLevel log3 = (LogLevel)Random.Shared.Next(1, 5);
    
    var routeKey = $"{log1}.{log2}.{log3}";
    
    string message = $"Log Type: {log1}-{log2}-{log3}";
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange: "logs-topic",
        routingKey: routeKey,
        mandatory: true,
        basicProperties: properties,
        body: body
    );

    Console.WriteLine($"Log gönderildi: {message}");
}

Console.ReadLine();