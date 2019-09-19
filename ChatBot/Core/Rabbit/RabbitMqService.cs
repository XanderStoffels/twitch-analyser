using ChatBot.Core.Rabbit.Exceptions;
using RabbitMQ.Client;

namespace ChatBot.Core.Rabbit
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

        public void Connect()
        {
            _connection = Factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("chat", false, false, false, null);
        }

        public void Disconnect()
        {
            _connection?.Close();
            _channel?.Close();
        }

        public void Publish(byte[] bytes)
        {
            if (!CanPublish())
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
            Disconnect();
            _connection = null;
            _channel = null;
        }
    }
}