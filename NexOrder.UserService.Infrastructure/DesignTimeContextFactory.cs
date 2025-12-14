using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NexOrder.UserService.Domain;
using System.IO;

namespace NexOrder.UserService.Infrastructure
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<UsersContext>
    {
        public UsersContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<UsersContext>();
            var connectionString = configuration.GetConnectionString("SystemDbConnectionString");

            // Explicitly set the migrations assembly
            optionsBuilder.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly("NexOrder.UserService.Infrastructure")
            );

            return new UsersContext(optionsBuilder.Options);
        }
    }
}
