using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Pizza
    {
        [Key]
        public required string PizzaId { get; set; }


        [ForeignKey(nameof(PizzaType))]
        public required string PizzaTypeId { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required]
        [StringLength(20)]
        public string Size { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 999.99)]
        public decimal Price { get; set; }

        public PizzaType PizzaType { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    }
}   