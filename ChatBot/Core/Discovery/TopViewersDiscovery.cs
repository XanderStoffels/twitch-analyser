using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api.Interfaces;

namespace ChatBot.Core.Discovery
{
    public class TopViewersDiscovery : IChannelDiscoverer
    {
        private readonly ITwitchAPI _api;

        public TopViewersDiscovery(ITwitchAPI api)
        {
            _api = api;
        }

        public async Task<List<string>> DiscoverChannels(int amount = 50)
        {
            if (amount > 50) amount = 50;
            
            // Get the top games on twitch by view count.
            var topGames = await GetTopGames(10);
            
            // Wait for all the streams to be fetched.
            var apiResults = await Task.WhenAll(topGames.Select(TopStreamersForGame));
            
            // Gather all the streams the API has returned.
            var streams = apiResults.SelectMany(s => s.ToList());
            
            // We want the top @amount of streams, sorted by viewer count.
            var topStreamers = streams.OrderByDescending(s => s.ViewerCount)
                .Take(amount)
                .Select(s => s.Id)
                .ToList();

            return topStreamers;
        }

        private async Task<List<string>> GetTopGames(int amount)
        {
            return (await _api.Games.helix.GetTopGamesAsync(first: amount))
                .Data
                .Select(d => d.Id)
                .ToList();
        }

        private async Task<List<(string Id, int ViewerCount)>> TopStreamersForGame(string gameId)
        {
            return (await _api.Streams.helix.GetStreamsAsync(gameIds: new[] {gameId}.ToList(), first: 5))
                .Streams
                .Select(s => (s.Id, s.ViewerCount))
                .ToList();
        }
    }
}