using System.Runtime.InteropServices.JavaScript;
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
// var routeKey = "*.Error.*";
var routeKey = "Info.#";
await channel.QueueBindAsync(randomQueueName, "logs-topic", routeKey);

consumer.ReceivedAsync += async (sender, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.Span);

    await Task.Delay(1500);

    Console.WriteLine("GELEN LOG: " + message);
    
    // File.AppendAllText("log.txt", message+ "\n");

    await channel.BasicAckAsync( deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(queue: randomQueueName, autoAck: false, consumer: consumer);

Console.ReadLine();