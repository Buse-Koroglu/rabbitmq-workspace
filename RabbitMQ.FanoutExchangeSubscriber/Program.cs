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

// EĞER KALICI BİR QUEUE OLUŞTURMAK İSTERSEK RANDOM QUEUE İSMİ VERMEYİ TERCİH ETMEYİZ.
 var queueDeclareResult = await channel.QueueDeclareAsync();
 var randomQueueName = queueDeclareResult.QueueName;

// Eğer kalıcı queue oluşturmak istersek 
// var queueName = "database-logs-queue";
// await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, false);

await channel.QueueBindAsync(
    queue: randomQueueName,
    exchange: "logs-fanout",
    routingKey: "",
    arguments: null
);

await channel.BasicQosAsync( prefetchSize: 0, prefetchCount: 1, global: false );

var consumer = new AsyncEventingBasicConsumer(channel);


consumer.ReceivedAsync += async (sender, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.Span);

    await Task.Delay(1500);

    Console.WriteLine("GELEN LOG: " + message);

    await channel.BasicAckAsync( deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(queue: randomQueueName, autoAck: false, consumer: consumer);

Console.ReadLine();