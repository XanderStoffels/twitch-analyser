using System;
using System.Threading;
using ChatBot.Config;
using ChatBot.Core.Discovery;
using ChatBot.Core.Rabbit;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace ChatBot.Core
{
    public class Controller
    {
        private readonly TwitchChatService _chatService;
        private readonly IRabbitMqService _rabbitService;
        private readonly CancellationTokenSource _cancelSource;

        public Controller(AppConfiguration config)
        {
            this._cancelSource = new CancellationTokenSource();
            
            // Configure client
            var credentials = new ConnectionCredentials(config.Username, config.AccessToken);
            var client = new TwitchClient();
            client.Initialize(credentials);
            
            // Configure API
            var api = new TwitchAPI();
            api.Settings.AccessToken = config.AccessToken;
            api.Settings.ClientId = config.ClientId;
            
            // Configure Chat Service
            IChannelDiscoverer discoverer = new TopViewersDiscovery(api);
            this._chatService = new TwitchChatService(client, discoverer, TimeSpan.FromMilliseconds(config.StreamPollingInterval));
            
            // Configure Rabbit MQ
            this._rabbitService = new RabbitMqService(config.RabbitMqChannel);
            
            // Configure events
            this._chatService.OnMessageSniffed +=
                async (sender, message) => await this._rabbitService.PublishTwitchChatMessage(message);

        }

        public void Start()
        {
            this._rabbitService.Connect();
            this._chatService.Start(this._cancelSource.Token);
        }

        public void Stop()
        {   
            this._cancelSource.Cancel();
        }

    }
}