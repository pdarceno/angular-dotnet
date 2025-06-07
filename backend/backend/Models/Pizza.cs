using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Pizza
    {
        [Key]
        public int PizzaId { get; set; }

        [Column(TypeName = "int")]
        public int PizzaTypeId { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string Size { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public PizzaType? PizzaType { get; set; }
    }
}   