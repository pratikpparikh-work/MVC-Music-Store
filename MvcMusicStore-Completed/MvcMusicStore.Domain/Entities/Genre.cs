using System.Collections.Generic;

namespace MvcMusicStore.Domain.Entities
{
    /// <summary>
    /// Represents a music genre/category.
    /// </summary>
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public virtual List<Album> Albums { get; set; } = new();
    }
}
