namespace ORS.Service.Dtos
{
    public class CustomerOrdersDto
    {
        public int CustomerId { get; set; }
        public List<OrdersDto> Orders { get; set; } = new List<OrdersDto>();
    }
}
