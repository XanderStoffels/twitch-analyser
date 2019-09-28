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
            var apiResults = await TopStreamersForGames(topGames, amount);
            
            var usernames = await IdToName(apiResults.Select(r => r.UserId).ToList());
            Console.WriteLine($"Total viewer count: {apiResults.Sum(s => s.ViewerCount)}");
            return usernames;
        }

        private async Task<List<string>> GetTopGames(int amount)
        {
            return (await _api.Games.helix.GetTopGamesAsync(first: amount))
                .Data
                .Select(d => d.Id)
                .ToList();
        }

        private async Task<List<(string Id, string UserId, int ViewerCount)>> TopStreamersForGames(List<string> gameIds, int amount)
        {
            return (await _api.Streams.helix.GetStreamsAsync(gameIds: gameIds, first: amount))
                .Streams
                .Select(s => (s.Id, s.UserId ,s.ViewerCount))
                .ToList();
        }

        private async Task<List<string>> IdToName(List<string> ids)
        {
            return (await _api.Users.helix.GetUsersAsync(ids)).Users.Select(u => u.DisplayName).ToList();
        }
    }
}