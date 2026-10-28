using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;

namespace MvcMusicStore.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of IAlbumRepository.
    /// </summary>
    public class AlbumRepository : IAlbumRepository
    {
        private readonly MusicStoreDbContext _context;

        public AlbumRepository(MusicStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Album>> GetAllAsync()
        {
            return await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .ToListAsync();
        }

        public async Task<Album?> GetByIdAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.AlbumId == id);
        }

        public async Task<IEnumerable<Album>> GetTopSellingAsync(int count)
        {
            return await _context.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .Include(a => a.OrderDetails)
                .OrderByDescending(a => a.OrderDetails.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Album> AddAsync(Album album)
        {
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            return album;
        }

        public async Task UpdateAsync(Album album)
        {
            _context.Entry(album).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
            }
        }
    }
}
