namespace backend.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public List<OrderDetailDto> Details { get; set; } = new();
    }
}
