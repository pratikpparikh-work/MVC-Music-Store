using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Artist data access.
    /// </summary>
    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(int id);
    }
}
