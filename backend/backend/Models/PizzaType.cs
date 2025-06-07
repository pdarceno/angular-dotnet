using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class PizzaType
    {
        [Key]
        public int PizzaTypeId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } = "";

        [Column(TypeName = "nvarchar(50)")]
        public string Category { get; set; } = "";

        [Column(TypeName = "nvarchar(255)")]
        public string Ingredients { get; set; } = "";

        public ICollection<Pizza>? Pizzas { get; set; }
    }
}