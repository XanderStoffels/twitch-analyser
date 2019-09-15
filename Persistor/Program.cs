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
        private static AppConfiguration Configuration;
        private static ManualResetEvent QuitEvent = new ManualResetEvent(false);
        
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                QuitEvent.Set();
                eventArgs.Cancel = true;
            };
            
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .Get<AppConfiguration>();

            MigrateDatabase();
            
            IRabbitMqService rabbitService = new RabbitMqService(Configuration.RabbitMqHost);
            rabbitService.Connect();
            rabbitService.OnReceive += SaveMessage;
 
            var reporting = new Timer(60000);
            reporting.Elapsed += (sender, eventArgs) => Report();
            reporting.Start();
            
            QuitEvent.WaitOne();

        }

        private static void SaveMessage(object sender, byte[] e)
        {
            var json = Encoding.UTF8.GetString(e);
            var message = JsonConvert.DeserializeObject<TwitchChatMessage>(json);
            using (var context = GetContext())
            {
                context.ChatMessages.Add(message);
                context.SaveChanges();
            }
        }

        private static void MigrateDatabase()
        {
            Console.WriteLine("Migrating database");
            PersistorDbContext.ConnectionString = Configuration.ConnectionString;
            using (var context = GetContext())
            {
                context.Database.Migrate();
            }
        }

        private static void Report()
        {
            using (var context = GetContext())
            {
                Console.WriteLine($"Amount of messages saved: {context.ChatMessages.Count()}");
            }
        }

        private static PersistorDbContext GetContext()
        {
            return new PersistorDbContext();
        }
    }
}
