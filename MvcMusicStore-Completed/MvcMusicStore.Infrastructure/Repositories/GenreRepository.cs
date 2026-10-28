using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;

namespace MvcMusicStore.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of IGenreRepository.
    /// </summary>
    public class GenreRepository : IGenreRepository
    {
        private readonly MusicStoreDbContext _context;

        public GenreRepository(MusicStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await _context.Genres
                .Include(g => g.Albums)
                .ThenInclude(a => a.Artist)
                .FirstOrDefaultAsync(g => g.Name == name);
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres
                .Include(g => g.Albums)
                .FirstOrDefaultAsync(g => g.GenreId == id);
        }
    }
}
