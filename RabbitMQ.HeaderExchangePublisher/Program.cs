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
await channel.ExchangeDeclareAsync("header-exchange",durable: true, type: ExchangeType.Headers );

var headers = new Dictionary<string, object?>{
    { "format", "pdf" },
    {"shape", "a4"}
};

var properties = new BasicProperties
{
    Persistent = true,
    Headers = headers
};

await channel.BasicPublishAsync("header-exchange", string.Empty, true, properties, Encoding.UTF8.GetBytes("Bu benim Header Exchange Örnek Mesajım.") );

Console.WriteLine("Mesaj Gönderilmişitir.");

Console.ReadLine();