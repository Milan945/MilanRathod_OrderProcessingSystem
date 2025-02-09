namespace ORS.Data.Models
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal TotalPrice => Product.Price * Quantity;
    }
}
