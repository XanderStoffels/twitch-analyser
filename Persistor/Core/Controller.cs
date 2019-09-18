using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Persistor.Core.Rabbit;
using Persistor.Data;

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