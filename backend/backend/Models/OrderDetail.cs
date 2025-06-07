using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }

        public required string PizzaId { get; set; }

        [Column(TypeName = "int")]
        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        [ForeignKey("PizzaId")]
        public Pizza Pizza { get; set; } = null!;
    }
}