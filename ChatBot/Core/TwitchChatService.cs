using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Core.Discovery;
using Shared;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;

namespace ChatBot.Core
{
    public class TwitchChatService
    {
        public event EventHandler<TwitchChatMessage> OnMessageSniffed;
        public int MaxConcurrentChannelJoins { get; set; } = 50;
        public TimeSpan SniffInterval { get; }

        private readonly ITwitchClient _client;
        private readonly IChannelDiscoverer _discoverer;
        
        public TwitchChatService(ITwitchClient client, IChannelDiscoverer discoverer, TimeSpan channelSniffInterval)
        {
            this._client = client;
            this._discoverer = discoverer;
            this.SniffInterval = channelSniffInterval;
            client.OnMessageReceived += ClientOnMessageReceived;
        }


        public void Start(CancellationToken token)
        {
            // Some nice Async/Await compatibility provided by the TwitchLib author...
            // /s
            this._client.OnConnected += HandleConnect;

            async void HandleConnect(object sender, OnConnectedArgs e)
            {
                this._client.OnConnected -= HandleConnect;
                await StartSniffing(token);
            }

            this._client.Connect();
        }

        private async Task StartSniffing(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var featuredChannels = await SniffChannels();
                var joinedChannels = this._client.JoinedChannels.Select(c => c.Channel).ToArray();

                // Join the new featured channels.
                foreach (var channel in featuredChannels.Except(joinedChannels))
                {
                    Console.WriteLine($"Joining {channel}");
                    this._client.JoinChannel(channel);
                }

                // Leave the channels that are not featured.
                foreach (var channel in joinedChannels.Except(featuredChannels))
                {
                    Console.WriteLine($"Leaving {channel}");
                    this._client.LeaveChannel(channel);
                }

                await Task.Delay(SniffInterval, token);
            }
        }

        private Task<List<string>> SniffChannels()
        {
            return this._discoverer.DiscoverChannels(this.MaxConcurrentChannelJoins);
        }

        private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var message = e.ChatMessage;
            var simpleMessage = new TwitchChatMessage
            {
                Channel = message.Channel,
                Message = message.Message,
                ReceivedAt = DateTime.Now,
                Username = message.Username,
                UserId = message.UserId
            };
            OnMessageSniffed?.Invoke(this, simpleMessage);
        }
    }
}