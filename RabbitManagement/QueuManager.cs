﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace RabbitManagement
{
    public class QueuManager : IDisposable
    {

        IConnection connection;
        IModel model;
        EventingBasicConsumer consumer;
        ConnectionFactory connectionFactory;
        public event EventHandler<BasicDeliverEventArgs> ReceiveMessage;


        public QueuManager(string login, string password, string host = "localhost")
        {
            connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = host;
            connectionFactory.UserName = login;
            connectionFactory.Password = password;
            connection = connectionFactory.CreateConnection();
        }

        public IConnection Connection { get => connection; }

        public void CreateExchangeFanout(string name, bool durable, IConnection connection)
        {
            using (model = connection.CreateModel())
            {
                model.ExchangeDeclare(name, ExchangeType.Fanout, durable);
            }
        }

        public void CreateExchangeTopic(string name, bool durable, IConnection connection)
        {
            using (model = connection.CreateModel())
            {
                model.ExchangeDeclare(name, ExchangeType.Topic, durable);
            }
        }

        public void CreateExchangeDirect(string name, bool durable, IConnection connection)
        {
            using (model = connection.CreateModel())
            {
                model.ExchangeDeclare(name, ExchangeType.Direct, durable);
            }
        }

        public void CreateExchangeHeaders(string name, bool durable, IConnection connection)
        {
            using (model = connection.CreateModel())
            {
                model.ExchangeDeclare(name, ExchangeType.Headers, durable, autoDelete: false);
            }
        }

        public void CreateQueue(string queueName, IConnection connection)
        {
            using (model = connection.CreateModel())
            {
                model.QueueDeclare(queueName, true, false, false);
            }
        }

        public void BindingQueue(string queueName, string exchangeName, IConnection connection, Dictionary<string, object> arguments = null, string routingKey = "")
        {
            if (arguments == null) arguments = new Dictionary<string, object>();

            using (model = connection.CreateModel())
            {
                model.QueueBind(queueName, exchangeName, routingKey, arguments);
            }
        }

        public void Enqueue(string message, IConnection connection, string exchangeName = "", string routungKey = "", Dictionary<string, object> properties = null)
        {
            using (model = connection.CreateModel())
            {
                IBasicProperties basicProperties = null;

                if (properties != null)
                {
                    basicProperties = model.CreateBasicProperties();
                    basicProperties.Headers = properties;
                }

                byte[] body = Encoding.UTF8.GetBytes(message);
                model.BasicPublish(exchangeName, routungKey, false, basicProperties, body);
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
