using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        [Column(TypeName = "int")]
        public int OrderId { get; set; }

        [Column(TypeName = "int")]
        public int PizzaId { get; set; }

        [Column(TypeName = "int")]
        public int Quantity { get; set; }

        public Order? Order { get; set; }
        public Pizza? Pizza { get; set; }
    }
}