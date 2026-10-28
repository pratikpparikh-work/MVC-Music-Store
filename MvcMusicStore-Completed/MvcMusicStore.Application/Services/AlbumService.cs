using System.Collections.Generic;
using System.Threading.Tasks;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;

namespace MvcMusicStore.Application.Services
{
    /// <summary>
    /// Application service for album operations.
    /// </summary>
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;

        public AlbumService(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }

        public async Task<IEnumerable<Album>> GetAllAlbumsAsync()
        {
            return await _albumRepository.GetAllAsync();
        }

        public async Task<Album?> GetAlbumByIdAsync(int id)
        {
            return await _albumRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Album>> GetTopSellingAlbumsAsync(int count)
        {
            return await _albumRepository.GetTopSellingAsync(count);
        }

        public async Task<Album> CreateAlbumAsync(Album album)
        {
            return await _albumRepository.AddAsync(album);
        }

        public async Task UpdateAlbumAsync(Album album)
        {
            await _albumRepository.UpdateAsync(album);
        }

        public async Task DeleteAlbumAsync(int id)
        {
            await _albumRepository.DeleteAsync(id);
        }
    }
}
