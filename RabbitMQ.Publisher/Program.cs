using System.Text;
using RabbitMQ.Client;
// rabbitmq bağlantı bilgileri
var factory = new ConnectionFactory{ HostName = "localhost", Port = 5672 , UserName = "guest", Password = "guest" };

// rabbitmq sunucusuna bağlanmak ve kanal açmak için
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

// queue olması gerektiği için bir queue tanımlıyorum
await channel.QueueDeclareAsync(queue:"Hello-Queue", durable: true, /* RabbitMQ yeniden başlarsa kuyruk silinmesin */ exclusive: false, autoDelete: false,arguments:null);
// autoDelete true olursa eğer o queueyu dinleyen bir tane bile consumer olmazsa queue'yu otomatik siler. exclusive true olursa queue'ya farklı servisler tarafından erişilemez.

var properties = new BasicProperties();
properties.Persistent = true;

foreach (var x in Enumerable.Range(1, 50))
{
    string message = "Message" + x;
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange: string.Empty, 
        routingKey: "Hello-Queue", 
        mandatory: true, 
        basicProperties: properties, 
        body: body
    );

    Console.WriteLine($"Mesaj gönderildi: {message}");
}

Console.ReadLine();