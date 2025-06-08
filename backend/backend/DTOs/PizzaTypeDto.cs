namespace backend.DTOs
{
    public class PizzaTypeDto
    {
        public string PizzaTypeId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Ingredients { get; set; } = "";
        public List<PizzaDto> Pizzas { get; set; } = new();
    }

}
