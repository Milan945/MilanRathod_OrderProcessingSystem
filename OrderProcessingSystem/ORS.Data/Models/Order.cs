namespace ORS.Data.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public bool IsFulfilled { get; set; }

        public decimal TotalPrice => OrderItems.Sum(oi => oi.TotalPrice);
    }
}