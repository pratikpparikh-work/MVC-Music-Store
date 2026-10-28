using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;

namespace MvcMusicStore.Application.Services
{
    /// <summary>
    /// Application service for artist operations.
    /// </summary>
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<Artist>> GetAllArtistsAsync()
        {
            return await _artistRepository.GetAllAsync();
        }

        public async Task<Artist?> GetArtistByIdAsync(int id)
        {
            return await _artistRepository.GetByIdAsync(id);
        }
    }
}
