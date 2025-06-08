using System.ComponentModel.DataAnnotations.Schema;

namespace backend.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal TotalPrice => Details.Sum(d => d.Price * d.Quantity);
        public int PizzaCount => Details.Sum(d => d.Quantity);
        public List<OrderDetailDto> Details { get; set; } = new();

    }
}
