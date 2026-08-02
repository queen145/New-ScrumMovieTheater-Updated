namespace ScrumMovieTheater.Models
{
    public class ConcessionItem
    {
        public int ConcessionItemId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Category { get; set; }

        public bool Active { get; set; } = true;

        public int LowStockThreshold { get; set; } = 10;

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}