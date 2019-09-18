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
    public class Controller
    {
        private readonly IRabbitMqService _rabbitMq;

        public Controller(IRabbitMqService rabbitMq)
        {
            this._rabbitMq = _rabbitMq;
            this._rabbitMq.OnReceive +=  async (sender, e) => await RabbitMqOnOnReceive(sender, e);
        }

        private async Task RabbitMqOnOnReceive(object sender, byte[] e)
        {
            var json = Encoding.UTF8.GetString(e);
            var message = JsonConvert.DeserializeObject<RabbitMqChatMessage>(json);

            // Keep the database updated before adding the chat message.
            using (var context = DbFactory.CreateDbContext())
            {
                await CheckForNewData(context, message);
                
            }
            
            
            
        }

        private static async Task CheckForNewData(PersistorDbContext context, RabbitMqChatMessage message)
        {
            var channelExist = await ChannelExist(context, message.Channel);
            if (!channelExist) await CreateChannel(context, message.Channel);

            var userExist = await UserExist(context, message.UserId);
            if (!userExist)
            {
                var user = (await CreateUser(context, message.UserId)).Entity;
                user.Usernames.Add(new TwitchUsername {FirstSeen = DateTime.Now, Username = message.Username});
            }
            else
            {
                var usernameExist =
                    await UsernameExistForUser(context, message.UserId, message.Username);
                if (!usernameExist)
                    await CreateUsernameForUser(context, message.UserId, message.Username);
            }

            if (context.HasUnsavedChanges()) await context.SaveChangesAsync();
        }

        private static ConfiguredTaskAwaitable<bool> ChannelExist(PersistorDbContext context, string id) =>
            context.Channels.AnyAsync(c => c.Id == id)
                .ConfigureAwait(false);

        private static ConfiguredTaskAwaitable<EntityEntry<Channel>> CreateChannel(PersistorDbContext context, string id) => 
            context.Channels.AddAsync(new Channel {Id = id, DiscoveredOn = DateTime.Now})
                .ConfigureAwait(false);

        private static ConfiguredTaskAwaitable<bool> UserExist(PersistorDbContext context, string id) =>
            context.Users.AnyAsync(c => c.Id == id).ConfigureAwait(false);

        private static ConfiguredTaskAwaitable<EntityEntry<TwitchUser>> CreateUser(PersistorDbContext context,
            string userId) =>
            context.Users.AddAsync(new TwitchUser {Id = userId}).ConfigureAwait(false);

        private static ConfiguredTaskAwaitable<bool> UsernameExistForUser(PersistorDbContext context, string userId,
            string username) =>
            context.Usernames.AnyAsync(u => u.UserId == userId && u.Username == username).ConfigureAwait(false);

        private static ConfiguredTaskAwaitable<EntityEntry<TwitchUsername>> CreateUsernameForUser(PersistorDbContext context,
            string userid, string username) =>
            context.Usernames.AddAsync(new TwitchUsername
                {FirstSeen = DateTime.Now, UserId = userid, Username = username}).ConfigureAwait(false);

    }
}