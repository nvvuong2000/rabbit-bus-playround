using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FormulaAirline.API.Services
{
    public class MessageProducer : IMessageProducer
    {
        public async void SendingMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
            };

            var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "bookings", durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object?> { { "x-queue-type", "quorum" } });

            var jsonString = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(jsonString);

            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "bookings", body: body);
            Console.WriteLine($" [x] Sent {message}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
    }
}
