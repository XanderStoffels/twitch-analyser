using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Internal;

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
        
        public bool HasUnsavedChanges()
        {
            return this.ChangeTracker.Entries().Any(e => e.State == EntityState.Added
                                                         || e.State == EntityState.Modified
                                                         || e.State == EntityState.Deleted);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
        }


    }
}

// "Server=minecraft.stream.army;Database=ChatHistory;User Id=sa;Password=DBpass123!"