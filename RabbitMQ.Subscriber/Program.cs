using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory{ HostName = "localhost", Port = 5672 , UserName = "guest", Password = "guest" };

// rabbitmq sunucusuna bağlanmak ve kanal açmak için
await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

// queue'yu zaten publisher tarafından tanımlamıştık 
// await channel.QueueDeclareAsync(queue:"Hello-Queue", durable: true, /* RabbitMQ yeniden başlarsa kuyruk silinmesin */ exclusive: false, autoDelete: false,arguments:null);

await channel.BasicQosAsync(0, 1, false); // BasicQosAsync ifadesi ile bir consumer'ın işlenmesi için alabileceği maksimum mesaj sayısını belirtmeye yarayan bir akış kontrolü sağlar. 
/* prefetchSize: bana herhangi boyutta bir mesaj gönderebilrsin. */
/* prefetchCount: her bir subscriber'a kaçar kaçar mesaj gelsin onun bilgisini verir */
/* global: true olursa prefetchCount'un tamamını subscriber sayısına göre dağıtır ancak false verirsek tüm subscriber'lara prefetchCount kadar gönderir. */

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (sender, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.Span);
    Thread.Sleep(1500);
    Console.WriteLine("GELEN MESAJ: " + message);
    await channel.BasicAckAsync(ea.DeliveryTag /* mesajın unique sıra id'si*/, multiple: false /* sadece teslim edilen mesajın queue'dan çıkarılacağını diğer mesajlara dokunulmayacağını garantiler.*/ );
};

await channel.BasicConsumeAsync("Hello-Queue", false, consumer);  // auto-ack ben sana mesajın sileneceğini haber edeceğim demektir.

Console.ReadLine();

