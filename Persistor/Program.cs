using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistor.Core;
using Persistor.Core.Data;
using Persistor.Core.Rabbit;

namespace Persistor
{
    internal class Program
    {
        private static readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                _quitEvent.Set();
                eventArgs.Cancel = true;
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var services = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<IRabbitMqService>(new RabbitMqService(config))
                .AddSingleton<IDataService, MongoDataService>()
                .AddTransient<IMessageHandler, MessageHandler>()
                .BuildServiceProvider();

            var c = new Controller(services);
            c.Start();

            _quitEvent.WaitOne();
        }
    }
}