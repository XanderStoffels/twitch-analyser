using System.Text;
using System.Threading.Tasks;
using ChatBot.Data;
using Newtonsoft.Json;

namespace ChatBot.Core.Rabbit
{
    public static class RabbitExtensions
    {
        public static Task PublishTwitchChatMessage(this IRabbitMqService service, TwitchChatMessage message)
        {
            return Task.Run(() => {            
                var json = JsonConvert.SerializeObject(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                service.Publish(bytes); 
            });
        }
    }
}