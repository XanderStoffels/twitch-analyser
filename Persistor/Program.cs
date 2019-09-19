using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistor.Config;
using Persistor.Core;
using Persistor.Core.Rabbit;
using Persistor.Data;

namespace Persistor
{
    internal class Program
    {
        private static AppConfiguration _configuration;
        private static readonly ManualResetEvent _quitEvent = new ManualResetEvent(false);

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                _quitEvent.Set();
                eventArgs.Cancel = true;
            };

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .Get<AppConfiguration>();

            DbFactory.Configure(_configuration.ConnectionString);
            MigrateDatabase();

            IRabbitMqService rabbitService = new RabbitMqService(_configuration.RabbitMqHost);
            IMessageHandler handler = new MessageHandler();
            var c = new Controller(rabbitService, handler);
            c.Start();

            _quitEvent.WaitOne();
        }

        private static void MigrateDatabase()
        {
            Console.WriteLine("Migrating database");
            using (var context = DbFactory.CreateDbContext())
            {
                context.Database.Migrate();
            }
        }
    }
}