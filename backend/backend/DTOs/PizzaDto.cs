namespace backend.DTOs
{
    public class PizzaDto
    {
        public string PizzaId { get; set; } = "";
        public string PizzaTypeName { get; set; } = "";
        public string Size { get; set; } = "";
        public decimal Price { get; set; }
    }
}
