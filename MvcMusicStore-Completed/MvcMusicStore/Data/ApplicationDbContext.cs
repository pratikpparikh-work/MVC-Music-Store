using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Models;

namespace MvcMusicStore.Data
{
    /// <summary>
    /// Application database context combining Identity and music store data.
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

            builder.Entity<Album>(entity =>
            {
                entity.HasKey(a => a.AlbumId);
                entity.Property(a => a.Price).HasColumnType("decimal(18,2)");
            });

            builder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.RecordId);
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
            });

            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.OrderDetailId);
                entity.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
            });
        }
    }
}
