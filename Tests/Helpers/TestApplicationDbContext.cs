using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Tests.Helpers
{
    /// <summary>
    /// SQLite-compatible ApplicationDbContext that skips PostgreSQL-specific model configuration.
    /// Used for unit testing with SQLite in-memory database.
    /// </summary>
    public class TestApplicationDbContext : ApplicationDbContext
    {
        public TestApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call the IdentityDbContext<ApplicationUser, IdentityRole, string> OnModelCreating
            // to configure Identity tables, but skip the Npgsql HasPostgresExtension call
            // We do this by calling the base of ApplicationDbContext's base class
            
            // Configure Identity tables manually (same as IdentityDbContext does)
            builder.Entity<ApplicationUser>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
                b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
                b.ToTable("AspNetUsers");
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
                b.Property(u => u.UserName).HasMaxLength(256);
                b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                b.Property(u => u.Email).HasMaxLength(256);
                b.Property(u => u.NormalizedEmail).HasMaxLength(256);
                b.HasMany<IdentityUserClaim<string>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
                b.HasMany<IdentityUserLogin<string>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
                b.HasMany<IdentityUserToken<string>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
                b.HasMany<IdentityUserRole<string>>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
            });

            builder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.HasKey(uc => uc.Id);
                b.ToTable("AspNetUserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.HasKey(l => new { l.LoginProvider, l.ProviderKey });
                b.Property(l => l.LoginProvider).HasMaxLength(128);
                b.Property(l => l.ProviderKey).HasMaxLength(128);
                b.ToTable("AspNetUserLogins");
            });

            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
                b.Property(t => t.LoginProvider).HasMaxLength(256);
                b.Property(t => t.Name).HasMaxLength(256);
                b.ToTable("AspNetUserTokens");
            });

            builder.Entity<IdentityRole>(b =>
            {
                b.HasKey(r => r.Id);
                b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
                b.ToTable("AspNetRoles");
                b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
                b.Property(u => u.Name).HasMaxLength(256);
                b.Property(u => u.NormalizedName).HasMaxLength(256);
                b.HasMany<IdentityUserRole<string>>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
                b.HasMany<IdentityRoleClaim<string>>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.HasKey(rc => rc.Id);
                b.ToTable("AspNetRoleClaims");
            });

            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.HasKey(r => new { r.UserId, r.RoleId });
                b.ToTable("AspNetUserRoles");
            });

            // Configure application entities (skip PostgreSQL-specific types)
            builder.Entity<Album>(entity =>
            {
                entity.HasKey(a => a.AlbumId);
            });

            builder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.RecordId);
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
            });

            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.OrderDetailId);
            });
        }
    }
}
