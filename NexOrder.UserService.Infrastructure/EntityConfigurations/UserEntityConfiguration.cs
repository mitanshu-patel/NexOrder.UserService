using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexOrder.UserService.Domain.Entities;

namespace NexOrder.UserService.Infrastructure.EntityConfigurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            if(builder is null)
            {
                throw new NotImplementedException();
            }

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(v => v.Email)
                   .IsRequired();
        }
    }
}
