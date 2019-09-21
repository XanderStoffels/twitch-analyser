using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchLib.Api.Interfaces;

namespace ChatBot.Core.Discovery
{
    public class FeaturedStreamsDiscoverer : IChannelDiscoverer
    {
        private readonly ITwitchAPI _twitchApi;

        public FeaturedStreamsDiscoverer(ITwitchAPI api)
        {
            this._twitchApi = api;
        }
        
        public async Task<List<string>> DiscoverChannels(int amount)
        {
            var features = await this._twitchApi.Streams.v5.GetFeaturedStreamAsync(amount);
            return features.Featured.Select(f => f.Stream.Channel.Name).ToList();
        }
    }
}