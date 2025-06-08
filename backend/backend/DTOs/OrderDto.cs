namespace backend.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public decimal TotalPrice => Details.Sum(d => d.Price * d.Quantity);
        public int PizzaCount => Details.Sum(d => d.Quantity);
        public List<OrderDetailDto> Details { get; set; } = new();

    }
}
