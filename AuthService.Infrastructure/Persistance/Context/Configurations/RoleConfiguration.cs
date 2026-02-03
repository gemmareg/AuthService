using AuthService.Domain;
using AuthService.Infrastructure.Persistance.Context.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistance.Context.Configurations
{
    public class RoleConfiguration : AuditableEntityConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);

            builder.ToTable("Claims");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(200);
        }
    }
}
