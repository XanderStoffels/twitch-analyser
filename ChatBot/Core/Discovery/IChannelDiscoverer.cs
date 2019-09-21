using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatBot.Core.Discovery
{
    public interface IChannelDiscoverer
    {
        Task<List<string>> DiscoverChannels(int amount);
    }
}