using System;
using Microsoft.Extensions.DependencyInjection;
using Persistor.Core.Data;
using Persistor.Core.Rabbit;
using Shared;

namespace Persistor.Core
{
    public class Controller
    {
        private readonly IRabbitMqService _rabbitMq;

        public Controller(IServiceProvider services)
        {
            var rabbitMq = services.GetRequiredService<IRabbitMqService>();

            _rabbitMq = rabbitMq;
            rabbitMq.OnReceive += (sender, e) => services.GetRequiredService<IMessageHandler>().Handle(e);
        }

        public void Start()
        {
            _rabbitMq.Connect();
        }
        
    }
}