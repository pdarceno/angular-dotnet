using backend.DTOs;
using backend.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetPizzas(int page = 1, int pageSize = 20)
        {
            // Build a lookup first (1 query)
            var totals = await _context.OrderDetails
                .GroupBy(od => od.PizzaId)
                .Select(g => new { PizzaId = g.Key, TotalSold = g.Sum(od => od.Quantity) })
                .ToDictionaryAsync(g => g.PizzaId, g => g.TotalSold);

            // Then project (1 query, no subqueries)
            var pizzas = await _context.Pizzas
                .Include(p => p.PizzaType)
                .OrderBy(p => p.PizzaId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PizzaDto
                {
                    PizzaId = p.PizzaId,
                    PizzaTypeName = p.PizzaType.Name,
                    Category = p.PizzaType.Category,
                    Size = p.Size,
                    Price = p.Price,
                    TotalSold = 0 // default, will be updated below
                })
                .ToListAsync();

            // Apply the lookup in memory
            foreach (var pizza in pizzas)
            {
                if (totals.TryGetValue(pizza.PizzaId, out var sold))
                    pizza.TotalSold = sold;
            }

            return Ok(pizzas);
        }

        // GET: api/Pizzas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaDto>> GetPizza(string id)
        {
            var pizza = await _context.Pizzas
                .Include(p => p.PizzaType)
                .FirstOrDefaultAsync(p => p.PizzaId == id);

            if (pizza == null)
                return NotFound();

            var totalSold = await _context.OrderDetails
                .Where(od => od.PizzaId == id)
                .SumAsync(od => (int?)od.Quantity) ?? 0;

            var dto = new PizzaDto
            {
                PizzaId = pizza.PizzaId,
                PizzaTypeName = pizza.PizzaType.Name,
                Category = pizza.PizzaType.Category,
                Size = pizza.Size,
                Price = pizza.Price,
                TotalSold = totalSold
            };

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPizzas(
        [FromQuery] string? pizzaTypeName = null,
        [FromQuery] string? category = null,
        [FromQuery] string? size = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            // Build lookup first
            var totals = await _context.OrderDetails
                .GroupBy(od => od.PizzaId)
                .Select(g => new { PizzaId = g.Key, TotalSold = g.Sum(od => od.Quantity) })
                .ToDictionaryAsync(g => g.PizzaId, g => g.TotalSold);

            // Base query with joins
            var query = _context.Pizzas
                .Include(p => p.PizzaType)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(pizzaTypeName))
                query = query.Where(p => p.PizzaType.Name.Contains(pizzaTypeName));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(p => p.PizzaType.Category == category);

            if (!string.IsNullOrWhiteSpace(size))
                query = query.Where(p => p.Size == size);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Project and paginate
            var pizzas = await query
                .OrderBy(p => p.PizzaId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PizzaDto
                {
                    PizzaId = p.PizzaId,
                    PizzaTypeName = p.PizzaType.Name,
                    Category = p.PizzaType.Category,
                    Size = p.Size,
                    Price = p.Price,
                    TotalSold = 0 // temporary
                })
                .ToListAsync();

            // Fill in totals
            foreach (var pizza in pizzas)
            {
                if (totals.TryGetValue(pizza.PizzaId, out var sold))
                    pizza.TotalSold = sold;
            }

            return Ok(pizzas);
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
