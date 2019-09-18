using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistor.Data
{
    public class PersistorDbContext : DbContext
    {
        public DbSet<TwitchUser> Users { get; set; }
        public DbSet<TwitchUsername> Usernames { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public PersistorDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
        }


    }
}

// "Server=minecraft.stream.army;Database=ChatHistory;User Id=sa;Password=DBpass123!"