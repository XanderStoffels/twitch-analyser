using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Persistor.Core.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
        private IModel _channel;

        private IConnection _connection;

        public RabbitMqService(string hostAddress)
        {
            Factory = new ConnectionFactory
            {
                HostName = hostAddress
            };
        }

        protected ConnectionFactory Factory { get; }
        public event EventHandler<byte[]> OnReceive;

        public void Connect()
        {
            _connection = Factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("chat", false, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => OnReceive?.Invoke(this, ea.Body);

            _channel.BasicConsume("chat", true, consumer);
        }

        public void Disconnect()
        {
            _connection?.Close();
            _channel?.Close();
        }


        public void Dispose()
        {
            Disconnect();
            _connection = null;
            _channel = null;
        }
    }
}