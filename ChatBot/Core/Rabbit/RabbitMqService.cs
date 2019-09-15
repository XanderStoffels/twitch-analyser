using System;
using ChatBot.Core.Rabbit.Exceptions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Serilog;

namespace ChatBot.Core.Rabbit
{
    public class RabbitMqService : IRabbitMqService
    {
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
        }

        public void Disconnect()
        {
            _connection?.Close();
            _channel?.Close();
        }

        public void Publish(byte[] bytes)
        {
            if (!this.CanPublish())
                throw new RabbitMqServiceException("The connection and/or channel is not open.");
            
            _channel.BasicPublish("", "chat", null, bytes);
        }

        public bool CanPublish()
        {
            if (_connection == null || _channel == null) return false;
            return _connection.IsOpen && _channel.IsOpen;
        }

        public void Dispose()
        {
            this.Disconnect();
            _connection = null;
            _channel = null;
        }
    }
}