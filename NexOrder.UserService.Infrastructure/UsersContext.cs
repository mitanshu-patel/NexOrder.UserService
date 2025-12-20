using Microsoft.EntityFrameworkCore;
using NexOrder.UserService.Application;
using NexOrder.UserService.Domain.Entities;

namespace NexOrder.UserService.Infrastructure
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(UsersContext).Assembly);
        }
    }
}
