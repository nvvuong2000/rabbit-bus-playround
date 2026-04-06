using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
};

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

 await channel.QueueDeclareAsync(queue: "bookings", durable: true, exclusive: false, autoDelete: false,
    arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

var consume = new AsyncEventingBasicConsumer(channel);

consume.ReceivedAsync += async (model, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"A message has been received: {message}");
};
await channel.BasicConsumeAsync("bookings", autoAck: true, consume);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
