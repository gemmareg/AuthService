using AuthService.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistance.Context.Configurations.Common
{
    public abstract class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : AuditableEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.CreatedBy)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.UpdatedAt);

            builder.Property(e => e.UpdatedBy)
                   .HasMaxLength(100);
        }
    }
}
