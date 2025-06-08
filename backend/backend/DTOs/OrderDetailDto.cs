namespace backend.DTOs
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public string PizzaName { get; set; } = "";
        public string Size { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
