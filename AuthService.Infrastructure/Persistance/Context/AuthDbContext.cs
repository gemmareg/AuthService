using AuthService.Domain;
using AuthService.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistance.Context
{
    public class AuthDbContext : DbContext
    {
        private const string EF_SYSTEM_USER = "System";

        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Token> Tokens => Set<Token>();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.SetCreated(EF_SYSTEM_USER);
                        break;
                    case EntityState.Modified:
                        entry.Property(x => x.CreatedAt).IsModified = false;
                        entry.Property(x => x.CreatedBy).IsModified = false;
                        entry.Entity.SetUpdated(EF_SYSTEM_USER);
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key and table mapping for entities
            modelBuilder.Entity<User>().ToTable("Users").HasKey(e => e.Id);
            modelBuilder.Entity<Role>().ToTable("Roles").HasKey(e => e.Id);
            modelBuilder.Entity<Permission>().ToTable("Permissions").HasKey(e => e.Id);
            modelBuilder.Entity<Token>().ToTable("Tokens").HasKey(e => e.Id);
                        
            // User relationships
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithMany(u => u.Roles);

            modelBuilder.Entity<Permission>()
                .HasMany(p => p.Users)
                .WithMany(u => u.Permissions);

            // Role relationships
            modelBuilder.Entity<Permission>()
                .HasMany(p => p.Roles)
                .WithMany(r => r.Permissions);

            // Token relationships
            modelBuilder.Entity<Token>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
