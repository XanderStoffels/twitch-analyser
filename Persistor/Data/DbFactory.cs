using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistor.Data
{
    public static class DbFactory
    {
        private static string _connectionString;

        public static PersistorDbContext CreateDbContext()
        {
            return CreateDbContext(_connectionString);
        }

        public static PersistorDbContext CreateDbContext(string connectionString)
        {
            _connectionString = connectionString;
            if (_connectionString == null)
                LoadConnectionString();

            var builder = new DbContextOptionsBuilder<PersistorDbContext>()
                .UseSqlServer(_connectionString)
                .EnableDetailedErrors();

            return new PersistorDbContext(builder.Options);

        }

        public static void Configure(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }
    }
}