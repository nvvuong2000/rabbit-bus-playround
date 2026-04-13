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

var exchange = "ticket.events";
var queue = "ticket.processing.queue";

await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct);

await channel.QueueDeclareAsync(queue, true, false, false);

await channel.QueueBindAsync(queue, exchange, "ticket.created");

Console.WriteLine("Waiting for ticket.created...");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    var message = Encoding.UTF8.GetString(ea.Body.ToArray());

    Console.WriteLine("Ticket Processing Service");
    Console.WriteLine(message);
};

await channel.BasicConsumeAsync(queue, true, consumer);

Console.ReadLine();
