using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Genre data access.
    /// </summary>
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre?> GetByNameAsync(string name);
        Task<Genre?> GetByIdAsync(int id);
    }
}
