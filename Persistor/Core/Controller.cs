using Persistor.Core.Rabbit;

namespace Persistor.Core
{
    public class Controller
    {
        private readonly IRabbitMqService _rabbitMq;
        public Controller(IRabbitMqService rabbitMq, IMessageHandler handler)
        {
            this._rabbitMq = rabbitMq;
            rabbitMq.OnReceive += (sender, e) => handler.Handle(e);
        }

        public void Start()
        {
            this._rabbitMq.Connect();
        }

    }
}