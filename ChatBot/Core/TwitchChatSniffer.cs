using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Data;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace ChatBot.Core
{
    public class TwitchChatSniffer
    {
        public event EventHandler<TwitchChatMessage> OnMessageSniffed;
        public int MaxConcurrentChannelJoins { get; set; } = 10;
        public TimeSpan SniffInterval { get; }
        protected ITwitchClient Client { get; private set; }
        protected ITwitchAPI Api { get; private set; }

        public TwitchChatSniffer(string username, string accessToken, string clientId, TimeSpan channelSniffInterval)
        {
            this.Client = new TwitchClient();
            this.Api = new TwitchAPI();
            this.SniffInterval = channelSniffInterval;

            var credentials = new ConnectionCredentials(username, accessToken);
            this.Client.Initialize(credentials);
            this.Client.OnMessageReceived += ClientOnMessageReceived;

            this.Api.Settings.ClientId = clientId;
            this.Api.Settings.AccessToken = accessToken;
        }

        public void Start(CancellationToken token)
        {
            // Some nice Async/Await compatibility provided by the TwitchLib author...
            // /s
            this.Client.OnConnected += HandleConnect;
            async void HandleConnect(object sender, OnConnectedArgs e)
            {
                this.Client.OnConnected -= HandleConnect;
                await StartSniffing(token);
            }

            this.Client.Connect();
        }

        private async Task StartSniffing(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var featuredChannels = await this.SniffChannels();
                var joinedChannels = this.Client.JoinedChannels.Select(c => c.Channel).ToArray();
                
                // Join the new featured channels.
                foreach (var channel in featuredChannels.Except(joinedChannels))
                {
                    Console.WriteLine($"Joining {channel}");
                    this.Client.JoinChannel(channel);
                }
                
                // Leave the channels that are not featured.
                foreach (var channel in joinedChannels.Except(featuredChannels))
                {
                    Console.WriteLine($"Leaving {channel}");
                    this.Client.LeaveChannel(channel);
                }
                
                await Task.Delay(this.SniffInterval, token);
            }
            
        }

        private async Task<List<string>> SniffChannels()
        {
            var features = await this.Api.Streams.v5.GetFeaturedStreamAsync(this.MaxConcurrentChannelJoins);
            return features.Featured.Select(f => f.Stream.Channel.Name).ToList();
        }

        private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var message = e.ChatMessage;;
            var simpleMessage = new TwitchChatMessage
            {
                Channel = message.Channel,
                Message = message.Message,
                ReceivedAt = DateTime.Now,
                Username = message.Username,
                UserId = message.UserId
            };
            this.OnMessageSniffed?.Invoke(this, simpleMessage);
        }
    }
}