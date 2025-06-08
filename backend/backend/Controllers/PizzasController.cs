using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzasController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public PizzasController(PizzaDbContext context)
        {
            _context = context;
        }

        // GET: api/Pizzas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pizza>>> GetPizzas()
        {
            return await _context.Pizzas.ToListAsync();
        }

        // GET: api/Pizzas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pizza>> GetPizza(int id)
        {
            var pizza = await _context.Pizzas.FindAsync(id);

            if (pizza == null)
            {
                return NotFound();
            }

            return pizza;
        }

        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var pizzas = new List<Pizza>();

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<PizzaCsvMap>();
                try
                {
                    pizzas = csv.GetRecords<Pizza>().ToList();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Invalid CSV format: {ex.Message}");
                }
            }

            await _context.Pizzas.AddRangeAsync(pizzas);
            await _context.SaveChangesAsync();

            return Ok(new { count = pizzas.Count });
        }
    }

    public sealed class PizzaCsvMap : ClassMap<Pizza>
    {
        public PizzaCsvMap()
        {
            Map(m => m.PizzaId).Name("pizza_id");
            Map(m => m.PizzaTypeId).Name("pizza_type_id");
            Map(m => m.Size).Name("size");
            Map(m => m.Price).Name("price");
        }
    }
}
