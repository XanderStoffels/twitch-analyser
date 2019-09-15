using System;
using ChatBot.Core.Rabbit;
using ChatBot.Core.Rabbit.Exceptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Persistor.Core.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
        public event EventHandler<byte[]> OnReceive; 
        protected ConnectionFactory Factory { get; }

        private IConnection _connection;
        private IModel _channel;

        public RabbitMqService(string hostAddress)
        {
            this.Factory = new ConnectionFactory()
            {
                HostName = hostAddress
            };
        }

        public void Connect()
        {
            this._connection = Factory.CreateConnection();
            this._channel = _connection.CreateModel();
            this._channel.QueueDeclare("chat", false, false, false, null);

            var consumer = new EventingBasicConsumer(this._channel);
            consumer.Received += (model, ea) => this.OnReceive?.Invoke(this, ea.Body);
            
            _channel.BasicConsume("chat", true, consumer);
        }

        public void Disconnect()
        {
            _connection?.Close();
            _channel?.Close();
        }
        

        public void Dispose()
        {
            this.Disconnect();
            _connection = null;
            _channel = null;
        }
    }
}