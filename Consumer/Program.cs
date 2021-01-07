using RabbitManagement;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {

        static IConnection connection;
        static IModel channel;
        static QueuManager _queuManager;
        static EventingBasicConsumer consumer;
        static ConnectionFactory connectionFactory;

        static void Main(string[] args)
        {
            _queuManager = new QueuManager("admin", "admin");

            string queueName = "fila4";

            using (var connection = _queuManager.Connection)
            using (channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = _queuManager.CreateConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicQos(0, 1, false);
                channel.BasicConsume(queue: "fila4", autoAck: false, consumer: consumer);
            }

            Console.ReadLine();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                using (channel = _queuManager.Connection.CreateModel())
                {
                    var body = e.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    channel.BasicAck(e.DeliveryTag, false);
                }
            }
            catch (Exception)
            {
                channel.BasicNack(e.DeliveryTag, false, false);
            }
        }
    }
}

