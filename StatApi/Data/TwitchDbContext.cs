using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace StatApi.Data
{
    public class TwitchDbContext : DbContext
    {
        public TwitchDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<TwitchUser> Users { get; set; }
        public DbSet<TwitchUsername> Usernames { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public bool HasUnsavedChanges()
        {
            return ChangeTracker.Entries().Any(e => e.State == EntityState.Added
                                                    || e.State == EntityState.Modified
                                                    || e.State == EntityState.Deleted);
        }
    }
}
