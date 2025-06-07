using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaApi.Models
{
    public class PizzaType
    {
        [Key]
        public int PizzaTypeId { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; } = "";

        [Column(TypeName = "varchar(50)")]
        public string Category { get; set; } = "";

        [Column(TypeName = "varchar(255)")]
        public string Ingredients { get; set; } = "";

        public ICollection<Pizza>? Pizzas { get; set; }
    }
}