using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Consumers
{
    public class UserRegisteredConsumer
    {
        public static void Consume()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "UserRegisteredQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");

                //Yahan pe Email Sending Logic implement kar sakte ho
            };

            channel.BasicConsume(queue: "UserRegisteredQueue", autoAck: true, consumer: consumer);

            Console.WriteLine("Waiting for messages.");
            Console.ReadLine();
        }
    }
}
