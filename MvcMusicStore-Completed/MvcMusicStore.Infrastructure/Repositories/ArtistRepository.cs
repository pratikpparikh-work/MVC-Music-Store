using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;
using MvcMusicStore.Infrastructure.Data;

namespace MvcMusicStore.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of IArtistRepository.
    /// </summary>
    public class ArtistRepository : IArtistRepository
    {
        private readonly MusicStoreDbContext _context;

        public ArtistRepository(MusicStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Artist>> GetAllAsync()
        {
            return await _context.Artists.ToListAsync();
        }

        public async Task<Artist?> GetByIdAsync(int id)
        {
            return await _context.Artists.FindAsync(id);
        }
    }
}
