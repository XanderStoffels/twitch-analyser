using Persistor.Core.Rabbit;

namespace Persistor.Core
{
    public class Controller
    {
        private readonly IRabbitMqService _rabbitMq;

        public Controller(IRabbitMqService rabbitMq)
        {
            this._rabbitMq = _rabbitMq;
        }
    }
}