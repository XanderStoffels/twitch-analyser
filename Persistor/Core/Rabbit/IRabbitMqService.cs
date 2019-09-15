using System;

namespace Persistor.Core.Rabbit
{
    public interface IRabbitMqService : IDisposable
    {
        event EventHandler<byte[]> OnReceive;
        void Connect();
        void Disconnect();
    }
}