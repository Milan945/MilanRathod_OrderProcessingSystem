namespace ORS.Service.Dtos
{
    public class CustomerOrdersDto
    {
        public int CustomerId { get; set; }
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}
