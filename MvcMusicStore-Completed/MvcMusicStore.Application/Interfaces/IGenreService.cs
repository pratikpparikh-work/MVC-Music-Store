using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Domain.Entities;

namespace MvcMusicStore.Application.Interfaces
{
    /// <summary>
    /// Service interface for genre-related business operations.
    /// </summary>
    public interface IGenreService
    {
        Task<IEnumerable<Genre>> GetAllGenresAsync();
        Task<Genre?> GetGenreByNameAsync(string name);
        Task<Genre?> GetGenreByIdAsync(int id);
    }
}
