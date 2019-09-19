using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Persistor.Core.Rabbit;
using Persistor.Data;

namespace Persistor.Core
{
    public class MessageHandler : IMessageHandler
    {
        public void Handle(byte[] e)
        {
            var json = Encoding.UTF8.GetString(e);
            var message = JsonConvert.DeserializeObject<RabbitMqChatMessage>(json);

            using (var context = DbFactory.CreateDbContext())
            {
                SaveNewData(context, message);
                SaveMessage(context, message);
                context.SaveChangesAsync();
            }
        }

        private void SaveNewData(PersistorDbContext context, RabbitMqChatMessage message)
        {
            var channelExist = ChannelExist(context, message.Channel);
            if (!channelExist)
            {
                CreateChannel(context, message.Channel);
                Console.WriteLine($"Discovered: Channel: {message.Channel}");
            }

            var userExist = UserExist(context, message.UserId);
            if (!userExist)
            {
                var user = CreateUser(context, message.UserId).Entity;
                user.Usernames.Add(new TwitchUsername {FirstSeen = DateTime.Now, Username = message.Username});
                Console.WriteLine($"Discovered: User: {user.Id}");
            }
            else
            {
                var usernameExist = UsernameExistForUser(context, message.UserId, message.Username);
                if (!usernameExist)
                {
                    CreateUsernameForUser(context, message.UserId, message.Username);
                    Console.WriteLine($"Discovered: Username for {message.UserId}: {message.Username}");
                }
            }

            if (context.HasUnsavedChanges()) context.SaveChanges();
        }

        private bool ChannelExist(PersistorDbContext context, string id) =>
            context.Channels.Any(c => c.Id == id);


        private void CreateChannel(PersistorDbContext context, string id) => 
            context.Channels.Add(new Channel {Id = id, DiscoveredOn = DateTime.Now});


        private bool UserExist(PersistorDbContext context, string id) =>
            context.Users.Any(c => c.Id == id);

        private EntityEntry<TwitchUser> CreateUser(PersistorDbContext context,
            string userId) =>
            context.Users.Add(new TwitchUser {Id = userId});

        private bool UsernameExistForUser(PersistorDbContext context, string userId,
            string username) =>
            context.Usernames.Any(u => u.UserId == userId && u.Username == username);

        private void CreateUsernameForUser(PersistorDbContext context,
            string userid, string username)
        {
            context.Usernames.Add(new TwitchUsername
                {FirstSeen = DateTime.Now, UserId = userid, Username = username});
        }

        private void SaveMessage(PersistorDbContext context,
            RabbitMqChatMessage message) =>
            context.ChatMessages.Add(new ChatMessage
            {
                ChannelId = message.Channel,
                SenderId = message.UserId,
                Message = message.Message,
                ReceivedOn = DateTime.Now
            });
    }
}