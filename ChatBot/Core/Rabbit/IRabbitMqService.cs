using System;

namespace ChatBot.Core.Rabbit
{
    public interface IRabbitMqService : IDisposable
    {
        void Connect();
        void Disconnect();
        void Publish(byte[] bytes);
        bool CanPublish();
    }
}