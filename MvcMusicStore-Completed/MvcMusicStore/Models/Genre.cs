using System.Collections.Generic;

namespace MvcMusicStore.Models
{
    public partial class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Album> Albums { get; set; } = new List<Album>();
    }
}
