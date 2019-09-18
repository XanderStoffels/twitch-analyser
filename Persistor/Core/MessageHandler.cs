using System;
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
        public async Task Handle(byte[] e)
        {
            var json = Encoding.UTF8.GetString(e);
            var message = JsonConvert.DeserializeObject<RabbitMqChatMessage>(json);

            using (var context = DbFactory.CreateDbContext())
            {
                await SaveNewData(context, message);
                await SaveMessage(context, message);
                await context.SaveChangesAsync();
                Console.WriteLine(message.Message);
            }
        }

        private async Task SaveNewData(PersistorDbContext context, RabbitMqChatMessage message)
        {
            var channelExist = await ChannelExist(context, message.Channel);
            if (!channelExist)
            {
                await CreateChannel(context, message.Channel);
                await context.SaveChangesAsync();
            }

            var userExist = await UserExist(context, message.UserId);
            if (!userExist)
            {
                var user = (await CreateUser(context, message.UserId)).Entity;
                user.Usernames.Add(new TwitchUsername {FirstSeen = DateTime.Now, Username = message.Username});
                await context.SaveChangesAsync();
            }
            else
            {
                var usernameExist =
                    await UsernameExistForUser(context, message.UserId, message.Username);
                if (!usernameExist)
                {
                    await CreateUsernameForUser(context, message.UserId, message.Username);
                    await context.SaveChangesAsync();

                }
            }
        }

        private ConfiguredTaskAwaitable<bool> ChannelExist(PersistorDbContext context, string id) =>
            context.Channels.AnyAsync(c => c.Id == id)
                .ConfigureAwait(false);

        private ConfiguredTaskAwaitable<EntityEntry<Channel>> CreateChannel(PersistorDbContext context, string id) =>
            context.Channels.AddAsync(new Channel {Id = id, DiscoveredOn = DateTime.Now})
                .ConfigureAwait(false);

        private ConfiguredTaskAwaitable<bool> UserExist(PersistorDbContext context, string id) =>
            context.Users.AnyAsync(c => c.Id == id).ConfigureAwait(false);

        private ConfiguredTaskAwaitable<EntityEntry<TwitchUser>> CreateUser(PersistorDbContext context,
            string userId) =>
            context.Users.AddAsync(new TwitchUser {Id = userId}).ConfigureAwait(false);

        private ConfiguredTaskAwaitable<bool> UsernameExistForUser(PersistorDbContext context, string userId,
            string username) =>
            context.Usernames.AnyAsync(u => u.UserId == userId && u.Username == username).ConfigureAwait(false);

        private ConfiguredTaskAwaitable<EntityEntry<TwitchUsername>> CreateUsernameForUser(PersistorDbContext context,
            string userid, string username) =>
            context.Usernames.AddAsync(new TwitchUsername
                {FirstSeen = DateTime.Now, UserId = userid, Username = username}).ConfigureAwait(false);

        private ConfiguredTaskAwaitable<EntityEntry<ChatMessage>> SaveMessage(PersistorDbContext context,
            RabbitMqChatMessage message) =>
            context.ChatMessages.AddAsync(new ChatMessage
            {
                ChannelId = message.Channel,
                SenderId = message.UserId,
                Message = message.Message,
                ReceivedOn = DateTime.Now
            }).ConfigureAwait(false);
    }
}