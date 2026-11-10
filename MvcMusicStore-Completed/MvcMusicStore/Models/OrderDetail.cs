using System.ComponentModel.DataAnnotations.Schema;

namespace MvcMusicStore.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int AlbumId { get; set; }
        public int Quantity { get; set; }

        // Use PostgreSQL numeric type for decimal columns
        [Column(TypeName = "numeric(18,2)")]
        public decimal UnitPrice { get; set; }

        public virtual Album? Album { get; set; }
        public virtual Order? Order { get; set; }
    }
}
