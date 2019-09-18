using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Persistor.Data
{
    public class DbFactory : IDesignTimeDbContextFactory<PersistorDbContext>
    {
        private string ConnectionString { get; set; }

        public DbFactory(string connectionString = null)
        {
            this.ConnectionString = connectionString;
        }

        public PersistorDbContext CreateDbContext(string[] args)
        {
            if (this.ConnectionString == null)
                LoadConnectionString();

            var builder = new DbContextOptionsBuilder<PersistorDbContext>()
                .UseSqlServer(this.ConnectionString)
                .EnableDetailedErrors();
            
            return new PersistorDbContext(builder.Options);
        }
                
        private void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            this.ConnectionString = configuration.GetConnectionString("ConnectionString");
        }
    }
}