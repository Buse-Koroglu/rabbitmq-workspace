using System.Text;
using RabbitMQ.Client;

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
    "logs-fanout",
    durable: true,
    type: ExchangeType.Fanout
);

var properties = new BasicProperties
{
    Persistent = true
};

foreach (var x in Enumerable.Range(1, 50))
{
    string message = "Log" + x;
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange: "logs-fanout",
        routingKey: "",
        mandatory: true,
        basicProperties: properties,
        body: body
    );

    Console.WriteLine($"Log gönderildi: {message}");
}

Console.ReadLine();