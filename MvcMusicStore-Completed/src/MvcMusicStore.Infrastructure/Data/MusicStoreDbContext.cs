using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Infrastructure.Data
{
    /// <summary>
    /// EF Core DbContext for MvcMusicStore - replaces legacy EF6 MusicStoreEntities.
    /// Inherits from IdentityDbContext to support ASP.NET Core Identity.
    /// </summary>
    public class MusicStoreDbContext : IdentityDbContext
    {
        public MusicStoreDbContext(DbContextOptions<MusicStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Artist> Artists { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Album configuration
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(a => a.AlbumId);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(160);
                entity.Property(a => a.AlbumArtUrl).HasMaxLength(1024);
                entity.Property(a => a.Price).HasColumnType("decimal(18,2)");
                entity.HasOne(a => a.Genre)
                      .WithMany(g => g.Albums)
                      .HasForeignKey(a => a.GenreId);
                entity.HasOne(a => a.Artist)
                      .WithMany(ar => ar.Albums)
                      .HasForeignKey(a => a.ArtistId);
            });

            // Genre configuration
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.GenreId);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(120);
                entity.Property(g => g.Description).HasMaxLength(500);
            });

            // Artist configuration
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(a => a.ArtistId);
                entity.Property(a => a.Name).IsRequired().HasMaxLength(120);
            });

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.RecordId);
                entity.Property(c => c.CartId).IsRequired().HasMaxLength(50);
                entity.HasOne(c => c.Album)
                      .WithMany()
                      .HasForeignKey(c => c.AlbumId);
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.Property(o => o.Total).HasColumnType("decimal(18,2)");
                entity.Property(o => o.Username).HasMaxLength(256);
                entity.Property(o => o.FirstName).IsRequired().HasMaxLength(160);
                entity.Property(o => o.LastName).IsRequired().HasMaxLength(160);
                entity.Property(o => o.Address).IsRequired().HasMaxLength(70);
                entity.Property(o => o.City).IsRequired().HasMaxLength(40);
                entity.Property(o => o.State).IsRequired().HasMaxLength(40);
                entity.Property(o => o.PostalCode).IsRequired().HasMaxLength(10);
                entity.Property(o => o.Country).IsRequired().HasMaxLength(40);
                entity.Property(o => o.Phone).IsRequired().HasMaxLength(24);
                entity.Property(o => o.Email).IsRequired().HasMaxLength(256);
            });

            // OrderDetail configuration
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.OrderDetailId);
                entity.Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
                entity.HasOne(od => od.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(od => od.OrderId);
                entity.HasOne(od => od.Album)
                      .WithMany(a => a.OrderDetails)
                      .HasForeignKey(od => od.AlbumId);
            });
        }
    }
}
