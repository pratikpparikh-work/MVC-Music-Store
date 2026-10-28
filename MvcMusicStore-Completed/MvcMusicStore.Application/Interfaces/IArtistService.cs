using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Application.Interfaces
{
    /// <summary>
    /// Service interface for artist-related business operations.
    /// </summary>
    public interface IArtistService
    {
        Task<IEnumerable<Artist>> GetAllArtistsAsync();
        Task<Artist?> GetArtistByIdAsync(int id);
    }
}
