using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class PizzaType
    {
        [Key]
        public required string PizzaTypeId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Column(TypeName = "nvarchar(50)")]
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "";

        [Column(TypeName = "nvarchar(255)")]
        [Required]
        [StringLength(255)]
        public string Ingredients { get; set; } = "";

        public ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>();
    }
}