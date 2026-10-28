using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Infrastructure.Data
{
    /// <summary>
    /// Seeds the database with initial sample data.
    /// Replaces the legacy EF6 DropCreateDatabaseIfModelChanges SampleData initializer.
    /// Updated for PostgreSQL 16 compatibility (DateTimeOffset for timestamptz columns).
    /// </summary>
    public static class SampleDataSeeder
    {
        public static void Seed(MusicStoreDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed roles
            if (!roleManager.RoleExistsAsync("Administrator").GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole("Administrator")).GetAwaiter().GetResult();
            }

            // Seed admin user
            if (userManager.FindByNameAsync("administrator@example.com").GetAwaiter().GetResult() == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "administrator@example.com",
                    Email = "administrator@example.com",
                    EmailConfirmed = true
                };
                var result = userManager.CreateAsync(adminUser, "Password123!").GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, "Administrator").GetAwaiter().GetResult();
                }
            }

            // Seed genres if empty
            if (!System.Linq.Enumerable.Any(context.Genres))
            {
                var genres = new List<Genre>
                {
                    new Genre { Name = "Rock", Description = "Rock music" },
                    new Genre { Name = "Jazz", Description = "Jazz music" },
                    new Genre { Name = "Metal", Description = "Metal music" },
                    new Genre { Name = "Alternative", Description = "Alternative music" },
                    new Genre { Name = "Disco", Description = "Disco music" },
                    new Genre { Name = "Blues", Description = "Blues music" },
                    new Genre { Name = "Latin", Description = "Latin music" },
                    new Genre { Name = "Reggae", Description = "Reggae music" },
                    new Genre { Name = "Pop", Description = "Pop music" },
                    new Genre { Name = "Classical", Description = "Classical music" }
                };
                context.Genres.AddRange(genres);
                context.SaveChanges();
            }

            // Seed artists if empty
            if (!System.Linq.Enumerable.Any(context.Artists))
            {
                var artists = new List<Artist>
                {
                    new Artist { Name = "AC/DC" },
                    new Artist { Name = "Accept" },
                    new Artist { Name = "Aerosmith" },
                    new Artist { Name = "Alice In Chains" },
                    new Artist { Name = "Alanis Morissette" },
                    new Artist { Name = "Amy Winehouse" },
                    new Artist { Name = "Audioslave" },
                    new Artist { Name = "Black Sabbath" },
                    new Artist { Name = "Deep Purple" },
                    new Artist { Name = "Foo Fighters" },
                    new Artist { Name = "Guns N' Roses" },
                    new Artist { Name = "Iron Maiden" },
                    new Artist { Name = "Led Zeppelin" },
                    new Artist { Name = "Metallica" },
                    new Artist { Name = "Nirvana" },
                    new Artist { Name = "Pearl Jam" },
                    new Artist { Name = "Pink Floyd" },
                    new Artist { Name = "Queen" },
                    new Artist { Name = "Red Hot Chili Peppers" },
                    new Artist { Name = "U2" }
                };
                context.Artists.AddRange(artists);
                context.SaveChanges();
            }

            // Seed albums if empty
            if (!System.Linq.Enumerable.Any(context.Albums))
            {
                var rockGenre = System.Linq.Enumerable.First(context.Genres, g => g.Name == "Rock");
                var metalGenre = System.Linq.Enumerable.First(context.Genres, g => g.Name == "Metal");
                var alternativeGenre = System.Linq.Enumerable.First(context.Genres, g => g.Name == "Alternative");
                var popGenre = System.Linq.Enumerable.First(context.Genres, g => g.Name == "Pop");

                var acdc = System.Linq.Enumerable.First(context.Artists, a => a.Name == "AC/DC");
                var metallica = System.Linq.Enumerable.First(context.Artists, a => a.Name == "Metallica");
                var nirvana = System.Linq.Enumerable.First(context.Artists, a => a.Name == "Nirvana");
                var u2 = System.Linq.Enumerable.First(context.Artists, a => a.Name == "U2");
                var queen = System.Linq.Enumerable.First(context.Artists, a => a.Name == "Queen");

                var albums = new List<Album>
                {
                    new Album { Title = "For Those About To Rock We Salute You", Genre = rockGenre, Price = 8.99M, Artist = acdc, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Let There Be Rock", Genre = rockGenre, Price = 8.99M, Artist = acdc, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "...And Justice For All", Genre = metalGenre, Price = 8.99M, Artist = metallica, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Black Album", Genre = metalGenre, Price = 8.99M, Artist = metallica, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Master Of Puppets", Genre = metalGenre, Price = 8.99M, Artist = metallica, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Nevermind", Genre = rockGenre, Price = 8.99M, Artist = nirvana, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Achtung Baby", Genre = rockGenre, Price = 8.99M, Artist = u2, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "The Best Of 1980-1990", Genre = rockGenre, Price = 8.99M, Artist = u2, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Greatest Hits I", Genre = rockGenre, Price = 8.99M, Artist = queen, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Greatest Hits II", Genre = rockGenre, Price = 8.99M, Artist = queen, AlbumArtUrl = "/images/placeholder.gif" },
                };
                context.Albums.AddRange(albums);
                context.SaveChanges();
            }
        }
    }
}
