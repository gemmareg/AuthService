using AuthService.Domain;
using AuthService.Infrastructure.Persistance.Context.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistance.Context.Configurations
{
    public class UserConfiguration : AuditableEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("Users");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Username)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(e => e.Email)
                   .IsRequired()
                   .HasMaxLength(500);
            builder.Property(e => e.IsActive)
                   .IsRequired();
        }
    }
}
