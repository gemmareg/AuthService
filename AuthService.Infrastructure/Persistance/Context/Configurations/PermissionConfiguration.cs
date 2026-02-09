using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Domain;
using Microsoft.EntityFrameworkCore;
using AuthService.Infrastructure.Persistance.Context.Configurations.Common;

namespace AuthService.Infrastructure.Persistance.Context.Configurations
{
    public class PermissionConfiguration : AuditableEntityConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            base.Configure(builder);

            builder.ToTable("Permissions");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(200);
            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(500);
        }
    }
}
