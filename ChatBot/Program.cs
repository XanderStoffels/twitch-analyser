using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Config;
using ChatBot.Core;
using ChatBot.Core.Rabbit;
using ChatBot.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using Serilog.Core;
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

            var rabbitService = new RabbitMqService(appConfiguration.RabbitMqChannel);
            rabbitService.Connect();

            var sniffer = new TwitchChatSniffer(appConfiguration.Username, appConfiguration.AccessToken,
                appConfiguration.ClientId, TimeSpan.FromMilliseconds(appConfiguration.StreamPollingInterval));
            sniffer.MaxConcurrentChannelJoins = appConfiguration.MaxConcurrentChannels;

            sniffer.OnMessageSniffed += (sender, message) =>
            {
                var json = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                rabbitService.Publish(bytes);
            };
            
            CancellationToken token = new CancellationToken();
            sniffer.Start(token);
            
            QuitEvent.WaitOne();
        }
        
        
        private static TwitchClient ConfiguredClient(string username, string accesstoken,  Action<ChatMessage> messageEvent)
        {
            var c = new TwitchClient();
            c.OnMessageReceived += (sender, receivedArgs) => { messageEvent.Invoke(receivedArgs.ChatMessage); };
            var cred = new ConnectionCredentials(username, accesstoken);
            c.Initialize(cred);
            return c;
        }
        
        
    }
}
