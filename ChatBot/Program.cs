using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Config;
using ChatBot.Core;
using ChatBot.Core.Rabbit;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace ChatBot
{
    internal class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        public static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                QuitEvent.Set();
                e.Cancel = true;
            };

            var appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .Get<AppConfiguration>();

            if (appConfiguration.StartupTimeout > 0)
            {
                Console.WriteLine("Waiting for startup timeout....");
                await Task.Delay(appConfiguration.StartupTimeout);
            }

            Controller controller = new Controller(appConfiguration);
            controller.Start();

            QuitEvent.WaitOne();
            controller.Stop();
        }
    }
}