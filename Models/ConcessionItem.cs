using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrumMovieTheater.Models
{
    [Table("concessionitems")]
    public class ConcessionItem
    {
        [Key]
        public int ConcessionItemId { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? Category { get; set; }

        public bool Active { get; set; }

        public int LowStockThreshold { get; set; }
    }
}