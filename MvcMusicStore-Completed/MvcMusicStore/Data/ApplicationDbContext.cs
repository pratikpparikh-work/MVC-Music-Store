using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Models;

namespace MvcMusicStore.Data
{
    /// <summary>
    /// Application database context combining Identity and music store data.
    /// Configured for PostgreSQL with snake_case naming conventions.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Artist> Artists { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Only apply PostgreSQL-specific configuration when using Npgsql provider
            var isPostgres = Database.ProviderName?.Contains("Npgsql") == true ||
                             Database.ProviderName?.Contains("PostgreSQL") == true;

            if (isPostgres)
            {
                // Set default schema to "public" for PostgreSQL
                builder.HasDefaultSchema("public");
                // Configure PostgreSQL extensions
                builder.HasPostgresExtension("uuid-ossp");
            }

            builder.Entity<Album>(entity =>
            {
                entity.HasKey(a => a.AlbumId);
                if (isPostgres)
                {
                    // Use PostgreSQL numeric type for decimal columns
                    entity.Property(a => a.Price).HasColumnType("numeric(18,2)");
                }
            });

            builder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.RecordId);
                if (isPostgres)
                {
                    // Map DateTime to timestamp without time zone for PostgreSQL
                    entity.Property(c => c.DateCreated).HasColumnType("timestamp without time zone");
                }
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                if (isPostgres)
                {
                    // Use PostgreSQL numeric type for decimal columns
                    entity.Property(o => o.Total).HasColumnType("numeric(18,2)");
                    // Map DateTime to timestamp without time zone for PostgreSQL
                    entity.Property(o => o.OrderDate).HasColumnType("timestamp without time zone");
                }
            });

            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.OrderDetailId);
                if (isPostgres)
                {
                    // Use PostgreSQL numeric type for decimal columns
                    entity.Property(od => od.UnitPrice).HasColumnType("numeric(18,2)");
                }
            });
        }
    }
}
