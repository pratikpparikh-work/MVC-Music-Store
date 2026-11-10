using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMusicStore.Models
{
    public class Cart
    {
        [Key]
        public int RecordId { get; set; }
        public string CartId { get; set; } = string.Empty;
        public int AlbumId { get; set; }
        public int Count { get; set; }

        // Map DateTime to timestamp without time zone for PostgreSQL
        [Column(TypeName = "timestamp without time zone")]
        public System.DateTime DateCreated { get; set; }

        public virtual Album? Album { get; set; }
    }
}
