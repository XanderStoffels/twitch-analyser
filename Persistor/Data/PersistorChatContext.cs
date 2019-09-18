using Microsoft.EntityFrameworkCore;

namespace Persistor.Data
{
    public class NewChatContext : DbContext
    {
        public DbSet<TwitchUser> Users { get; set; }
        public DbSet<TwitchUsername> Usernames { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseSqlServer(
                    "Server=minecraft.stream.army;Database=ChatHistory;User Id=sa;Password=DBpass123!");
            }

            ;
        }
    }
}