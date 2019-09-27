using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using Persistor.Core.Data;
using Shared;

namespace Persistor.Core
{
    public class MessageHandler : IMessageHandler
    {
        private readonly IDataService _dataService;

        public MessageHandler(IDataService dataService)
        {
            this._dataService = dataService;
        }
        public void Handle(byte[] e)
        {
            Task.Run(async () =>
            {
                var json = Encoding.UTF8.GetString(e);
                var message = JsonSerializer.Deserialize<TwitchChatMessage>(json);
                await this._dataService.SaveMessageAsync(Convert(message));
            });
        }
        
        private static MessageModel Convert(TwitchChatMessage message) => new MessageModel()
        {
            Message = message.Message,
            Username = message.Username,
            Channel = message.Channel,
            ReceivedAt = message.ReceivedAt,
            UserId = message.UserId
        };
    }
}