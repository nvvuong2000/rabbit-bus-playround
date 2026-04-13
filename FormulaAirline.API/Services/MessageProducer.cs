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
            var exchangeName = "ticket.events";
            Console.WriteLine("Enter event type:");
            Console.WriteLine("1 = ticket.created");
            Console.WriteLine("2 = ticket.cancelled");
            Console.WriteLine("3 = ticket.updated");

            var option = Console.ReadLine();

            string routingKey = option switch
            {
                "1" => "ticket.created",
                "2" => "ticket.cancelled",
                "3" => "ticket.updated",
                _ => "ticket.created"
            };

            var ticketEvent = new
            {
                TicketId = Guid.NewGuid().ToString(),
                Passenger = "Vuong",
                Route = "SGN-HAN",
                CreatedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(ticketEvent);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: false);

            var jsonString = JsonSerializer.Serialize(message);


            await channel.BasicPublishAsync(exchange: exchangeName, routingKey: routingKey, body: body);
            Console.WriteLine($" [x] Sent {message}, messgae {routingKey}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }
    }
}
