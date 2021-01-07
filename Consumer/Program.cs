using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {

        static IConnection connection;
        static IModel model;
        static EventingBasicConsumer consumer;
        static ConnectionFactory connectionFactory;

        static void Main(string[] args)
        {

            string queueName = "fila4";

            connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "admin";
            connectionFactory.Password = "admin";
            connection = connectionFactory.CreateConnection();

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    channel.BasicNack(ea.DeliveryTag, false, false);
                };

                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
