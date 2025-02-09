using System.Text.Json.Serialization;

namespace ORS.Data.Models
{
    public class Order
    {
        public int Id { get; set; } // Primary key  
        public int CustomerId { get; set; } // Foreign key for Customer  
        [JsonIgnore]
        public Customer Customer { get; set; } = null!; // Navigation property  
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Navigation property  
        public bool IsFulfilled { get; set; } // Tracks whether the order is fulfilled  

        public DateTime OrderDate { get; set; } // Stores the date and time the order was created  

        public decimal TotalPrice => OrderItems.Sum(oi => oi.TotalPrice); // Computed property  
    }
}