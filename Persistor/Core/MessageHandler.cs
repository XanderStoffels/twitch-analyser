using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using MongoDB.Driver;
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

        public void Handle(byte[] e) => HandleAsync(e);

        private void HandleSync(byte[] e)
        {
            var json = Encoding.UTF8.GetString(e);
            var message = JsonSerializer.Deserialize<TwitchChatMessage>(json);
            try
            {
                this._dataService.SaveMessage(Convert(message));
            }
            catch (MongoAuthenticationException exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        private async Task HandleAsync(byte[] e)
        {
            var json = Encoding.UTF8.GetString(e);
            var message = JsonSerializer.Deserialize<TwitchChatMessage>(json);
            try
            {
                await this._dataService.SaveMessageAsync(Convert(message)).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw;
            }
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