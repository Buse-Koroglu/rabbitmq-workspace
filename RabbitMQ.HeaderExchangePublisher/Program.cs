using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.SharedClassForHeaderExchange;

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

var product = new Product{Id = 342,Name = "Trileçe",Price = 125,Stock = 15};
var productJsonString = JsonSerializer.Serialize(product);

await channel.BasicPublishAsync("header-exchange", string.Empty, true, properties,Encoding.UTF8.GetBytes(productJsonString) );

Console.WriteLine("Mesaj Gönderilmişitir.");

Console.ReadLine();