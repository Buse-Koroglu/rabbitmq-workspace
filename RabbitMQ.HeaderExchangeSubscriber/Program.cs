using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.BasicQosAsync( prefetchSize: 0, prefetchCount: 1, global: false );

var consumer = new AsyncEventingBasicConsumer(channel);

var queueDeclareResult = await channel.QueueDeclareAsync();
var randomQueueName = queueDeclareResult.QueueName;

var headers = new Dictionary<string, object?>{
    { "format4", "pdf" },
    {"shape", "a4"},
    {"x-match", "any"}
};

await channel.ExchangeDeclareAsync("header-exchange",durable: true, type: ExchangeType.Headers );

await channel.QueueBindAsync(randomQueueName, "header-exchange",string.Empty,headers);

consumer.ReceivedAsync += async (sender, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.Span);

    await Task.Delay(1500);

    Console.WriteLine("GELEN MESAJ: " + message);
    
    await channel.BasicAckAsync( deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(queue: randomQueueName, autoAck: false, consumer: consumer);

Console.ReadLine();