using MvcMusicStore.Application.DTOs;
using MvcMusicStore.Application.Interfaces;
using MvcMusicStore.Domain.Entities;
using MvcMusicStore.Domain.Interfaces;

namespace MvcMusicStore.Application.Services
{
    /// <summary>
    /// Album service implementation.
    /// </summary>
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IArtistRepository _artistRepository;

        public AlbumService(IAlbumRepository albumRepository, IGenreRepository genreRepository, IArtistRepository artistRepository)
        {
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<AlbumDto>> GetTopSellingAlbumsAsync(int count)
        {
            var albums = await _albumRepository.GetTopSellingAlbumsAsync(count);
            return albums.Select(MapToDto);
        }

        public async Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync()
        {
            var albums = await _albumRepository.GetAlbumsWithDetailsAsync();
            return albums.Select(MapToDto);
        }

        public async Task<AlbumDto?> GetAlbumByIdAsync(int id)
        {
            var album = await _albumRepository.GetAlbumWithDetailsAsync(id);
            return album == null ? null : MapToDto(album);
        }

        public async Task<AlbumDto> CreateAlbumAsync(AlbumDto albumDto)
        {
            var album = MapToEntity(albumDto);
            await _albumRepository.AddAsync(album);
            await _albumRepository.SaveChangesAsync();
            albumDto.AlbumId = album.AlbumId;
            return albumDto;
        }

        public async Task UpdateAlbumAsync(AlbumDto albumDto)
        {
            var album = await _albumRepository.GetByIdAsync(albumDto.AlbumId);
            if (album == null) throw new KeyNotFoundException($"Album {albumDto.AlbumId} not found.");

            album.Title = albumDto.Title;
            album.Price = albumDto.Price;
            album.AlbumArtUrl = albumDto.AlbumArtUrl;
            album.GenreId = albumDto.GenreId;
            album.ArtistId = albumDto.ArtistId;

            _albumRepository.Update(album);
            await _albumRepository.SaveChangesAsync();
        }

        public async Task DeleteAlbumAsync(int id)
        {
            var album = await _albumRepository.GetByIdAsync(id);
            if (album == null) throw new KeyNotFoundException($"Album {id} not found.");
            _albumRepository.Remove(album);
            await _albumRepository.SaveChangesAsync();
        }

        private static AlbumDto MapToDto(Album album) => new AlbumDto
        {
            AlbumId = album.AlbumId,
            GenreId = album.GenreId,
            ArtistId = album.ArtistId,
            Title = album.Title,
            Price = album.Price,
            AlbumArtUrl = album.AlbumArtUrl,
            GenreName = album.Genre?.Name,
            ArtistName = album.Artist?.Name
        };

        private static Album MapToEntity(AlbumDto dto) => new Album
        {
            AlbumId = dto.AlbumId,
            GenreId = dto.GenreId,
            ArtistId = dto.ArtistId,
            Title = dto.Title,
            Price = dto.Price,
            AlbumArtUrl = dto.AlbumArtUrl
        };
    }

    /// <summary>
    /// Genre service implementation.
    /// </summary>
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<GenreDto>> GetAllGenresAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            return genres.Select(g => new GenreDto
            {
                GenreId = g.GenreId,
                Name = g.Name,
                Description = g.Description
            });
        }

        public async Task<GenreDto?> GetGenreByNameAsync(string name)
        {
            var genre = await _genreRepository.GetGenreByNameAsync(name);
            if (genre == null) return null;
            return new GenreDto { GenreId = genre.GenreId, Name = genre.Name, Description = genre.Description };
        }

        public async Task<GenreDto?> GetGenreWithAlbumsAsync(string name)
        {
            var genre = await _genreRepository.GetGenreWithAlbumsAsync(name);
            if (genre == null) return null;
            return new GenreDto
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Description = genre.Description,
                Albums = genre.Albums.Select(a => new AlbumDto
                {
                    AlbumId = a.AlbumId,
                    Title = a.Title,
                    Price = a.Price,
                    AlbumArtUrl = a.AlbumArtUrl,
                    ArtistName = a.Artist?.Name
                }).ToList()
            };
        }
    }

    /// <summary>
    /// Artist service implementation.
    /// </summary>
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistService(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<IEnumerable<ArtistDto>> GetAllArtistsAsync()
        {
            var artists = await _artistRepository.GetAllAsync();
            return artists.Select(a => new ArtistDto { ArtistId = a.ArtistId, Name = a.Name });
        }
    }

    /// <summary>
    /// Shopping cart service implementation.
    /// Replaces legacy ShoppingCart model with proper service layer.
    /// Session management is handled in the Web layer via IHttpContextAccessor.
    /// </summary>
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IOrderRepository _orderRepository;

        public ShoppingCartService(
            ICartRepository cartRepository,
            IAlbumRepository albumRepository,
            IOrderDetailRepository orderDetailRepository,
            IOrderRepository orderRepository)
        {
            _cartRepository = cartRepository;
            _albumRepository = albumRepository;
            _orderDetailRepository = orderDetailRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ShoppingCartDto> GetCartAsync(string cartId)
        {
            var cartItems = await _cartRepository.GetCartItemsAsync(cartId);
            var total = await _cartRepository.GetCartTotalAsync(cartId);

            return new ShoppingCartDto
            {
                CartItems = cartItems.Select(c => new CartItemDto
                {
                    RecordId = c.RecordId,
                    CartId = c.CartId,
                    AlbumId = c.AlbumId,
                    Count = c.Count,
                    DateCreated = c.DateCreated,
                    AlbumTitle = c.Album?.Title ?? string.Empty,
                    AlbumPrice = c.Album?.Price ?? 0,
                    AlbumArtUrl = c.Album?.AlbumArtUrl ?? string.Empty
                }).ToList(),
                CartTotal = total
            };
        }

        public async Task AddToCartAsync(string cartId, int albumId)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(cartId, albumId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    AlbumId = albumId,
                    CartId = cartId,
                    Count = 1,
                    DateCreated = DateTime.UtcNow
                };
                await _cartRepository.AddAsync(cartItem);
            }
            else
            {
                cartItem.Count++;
                _cartRepository.Update(cartItem);
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task<int> RemoveFromCartAsync(string cartId, int recordId)
        {
            var cartItem = await _cartRepository.GetCartItemByRecordIdAsync(cartId, recordId);
            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                    _cartRepository.Update(cartItem);
                }
                else
                {
                    _cartRepository.Remove(cartItem);
                }
                await _cartRepository.SaveChangesAsync();
            }

            return itemCount;
        }

        public async Task<int> GetCartCountAsync(string cartId)
        {
            return await _cartRepository.GetCartCountAsync(cartId);
        }

        public async Task<decimal> GetCartTotalAsync(string cartId)
        {
            return await _cartRepository.GetCartTotalAsync(cartId);
        }

        public async Task MigrateCartAsync(string oldCartId, string newCartId)
        {
            await _cartRepository.MigrateCartAsync(oldCartId, newCartId);
        }

        public async Task<int> CreateOrderAsync(string cartId, Order order)
        {
            decimal orderTotal = 0;
            var cartItems = await _cartRepository.GetCartItemsAsync(cartId);

            // First save the order to get the OrderId
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album?.Price ?? 0,
                    Quantity = item.Count
                };
                orderTotal += item.Count * (item.Album?.Price ?? 0);
                await _orderDetailRepository.AddAsync(orderDetail);
            }

            order.Total = orderTotal;
            await _orderRepository.SaveChangesAsync();

            await _cartRepository.EmptyCartAsync(cartId);

            return order.OrderId;
        }

        public async Task<string?> GetAlbumTitleByCartRecordIdAsync(string cartId, int recordId)
        {
            var cartItem = await _cartRepository.GetCartItemByRecordIdAsync(cartId, recordId);
            return cartItem?.Album?.Title;
        }
    }

    /// <summary>
    /// Order service implementation.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<bool> ValidateOrderOwnerAsync(int orderId, string username)
        {
            return await _orderRepository.ValidateOrderOwnerAsync(orderId, username);
        }
    }
}
