using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class RabbitMQService
    {
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;

        public RabbitMQService(IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQ");
            _hostname = rabbitMQConfig["HostName"];
            _username = rabbitMQConfig["UserName"];
            _password = rabbitMQConfig["Password"];
        }

        public void PublishMessage(string queueName, string message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };


            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

            Console.WriteLine($"Sent: {message}");
        }
    }
}
