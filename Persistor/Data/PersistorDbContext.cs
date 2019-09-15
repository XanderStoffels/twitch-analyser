using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistor.Data
{
    public class PersistorDbContext : DbContext
    {
        public static string ConnectionString =
            "";
        public DbSet<TwitchChatMessage> ChatMessages { get; set; }

        public PersistorDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
    }
}