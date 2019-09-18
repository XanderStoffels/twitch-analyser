using System;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Persistor.Config;
using Persistor.Core.Rabbit;
using Persistor.Data;
using Timer = System.Timers.Timer;

namespace Persistor
{
    class Program
    {
        private static AppConfiguration _configuration;
        private static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        
        static void Main(string[] args)
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
            Controller
 
          
            
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
