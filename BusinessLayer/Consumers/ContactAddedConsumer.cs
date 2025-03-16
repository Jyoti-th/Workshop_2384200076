using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Consumers
{
    public class ContactAddedConsumer
    {
        public static void Consume()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "ContactAddedQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" Received: {message}");
            };

            channel.BasicConsume(queue: "ContactAddedQueue", autoAck: true, consumer: consumer);

            Console.WriteLine(" [Consumer Started] Listening for Contact Added Events...");
            Console.ReadLine();
        }
    }
}
