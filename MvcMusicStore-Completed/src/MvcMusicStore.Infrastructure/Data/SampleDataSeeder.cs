using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Infrastructure.Data
{
    /// <summary>
    /// Seeds the database with initial sample data.
    /// Replaces the legacy EF6 DropCreateDatabaseIfModelChanges SampleData initializer.
    /// </summary>
    public static class SampleDataSeeder
    {
        public static async Task SeedAsync(MusicStoreDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created and migrations applied
            await context.Database.EnsureCreatedAsync();

            // Seed roles
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                await roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            // Seed admin user
            if (await userManager.FindByNameAsync("Administrator") == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "Administrator",
                    Email = "admin@musicstore.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }
            }

            // Seed genres if none exist
            if (!await context.Genres.AnyAsync())
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
                await context.Genres.AddRangeAsync(genres);
                await context.SaveChangesAsync();
            }

            // Seed artists if none exist
            if (!await context.Artists.AnyAsync())
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
                    new Artist { Name = "Ozzy Osbourne" },
                    new Artist { Name = "Pearl Jam" },
                    new Artist { Name = "Pink Floyd" },
                    new Artist { Name = "Queen" },
                    new Artist { Name = "Red Hot Chili Peppers" },
                    new Artist { Name = "Santana" },
                    new Artist { Name = "Soundgarden" },
                    new Artist { Name = "The Rolling Stones" },
                    new Artist { Name = "U2" },
                    new Artist { Name = "Van Halen" }
                };
                await context.Artists.AddRangeAsync(artists);
                await context.SaveChangesAsync();
            }

            // Seed albums if none exist
            if (!await context.Albums.AnyAsync())
            {
                var rockGenre = await context.Genres.FirstAsync(g => g.Name == "Rock");
                var metalGenre = await context.Genres.FirstAsync(g => g.Name == "Metal");
                var alternativeGenre = await context.Genres.FirstAsync(g => g.Name == "Alternative");
                var popGenre = await context.Genres.FirstAsync(g => g.Name == "Pop");

                var acdc = await context.Artists.FirstAsync(a => a.Name == "AC/DC");
                var metallica = await context.Artists.FirstAsync(a => a.Name == "Metallica");
                var nirvana = await context.Artists.FirstAsync(a => a.Name == "Nirvana");
                var u2 = await context.Artists.FirstAsync(a => a.Name == "U2");
                var queen = await context.Artists.FirstAsync(a => a.Name == "Queen");
                var pinkFloyd = await context.Artists.FirstAsync(a => a.Name == "Pink Floyd");
                var ledZeppelin = await context.Artists.FirstAsync(a => a.Name == "Led Zeppelin");
                var ironMaiden = await context.Artists.FirstAsync(a => a.Name == "Iron Maiden");
                var blackSabbath = await context.Artists.FirstAsync(a => a.Name == "Black Sabbath");
                var gunsNRoses = await context.Artists.FirstAsync(a => a.Name == "Guns N' Roses");

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
                    new Album { Title = "News Of The World", Genre = rockGenre, Price = 8.99M, Artist = queen, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Dark Side Of The Moon", Genre = rockGenre, Price = 8.99M, Artist = pinkFloyd, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Led Zeppelin IV", Genre = rockGenre, Price = 8.99M, Artist = ledZeppelin, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "The Number of The Beast", Genre = metalGenre, Price = 8.99M, Artist = ironMaiden, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Black Sabbath", Genre = metalGenre, Price = 8.99M, Artist = blackSabbath, AlbumArtUrl = "/images/placeholder.gif" },
                    new Album { Title = "Appetite for Destruction", Genre = rockGenre, Price = 8.99M, Artist = gunsNRoses, AlbumArtUrl = "/images/placeholder.gif" }
                };
                await context.Albums.AddRangeAsync(albums);
                await context.SaveChangesAsync();
            }
        }
    }
}
