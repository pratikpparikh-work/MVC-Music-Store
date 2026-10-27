using System.ComponentModel.DataAnnotations;

namespace MvcMusicStore.Domain.Entities
{
    /// <summary>
    /// Represents a shopping cart item.
    /// </summary>
    public class Cart
    {
        [Key]
        public int RecordId { get; set; }
        public string CartId { get; set; } = string.Empty;
        public int AlbumId { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual Album? Album { get; set; }
    }
}
