using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using CsvHelper;
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzaTypesController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public PizzaTypesController(PizzaDbContext context)
        {
            _context = context;
        }

        // GET: api/PizzaTypes
        [HttpGet]
        public async Task<IActionResult> GetPizzaTypes(int page = 1, int pageSize = 20)
        {
            var totals = await _context.OrderDetails
            .GroupBy(od => od.PizzaId)
            .Select(g => new { PizzaId = g.Key, TotalSold = g.Sum(od => od.Quantity) })
            .ToDictionaryAsync(g => g.PizzaId, g => g.TotalSold);

            var pizzaTypes = await _context.PizzaTypes
            .Include(pt => pt.Pizzas)
            .OrderBy(p => p.PizzaTypeId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var result = pizzaTypes.Select(pt => new PizzaTypeDto
            {
                PizzaTypeId = pt.PizzaTypeId,
                Name = pt.Name,
                Category = pt.Category,
                Ingredients = pt.Ingredients,
                Pizzas = pt.Pizzas.Select(pi => new PizzaDto
                {
                    PizzaId = pi.PizzaId,
                    Size = pi.Size,
                    Price = pi.Price,
                    TotalSold = totals.TryGetValue(pi.PizzaId, out var sold) ? sold : 0
                }).ToList()
            }).ToList();

            return Ok(result);
        }


        // GET: api/PizzaTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaTypeDto>> GetPizzaType(string id)
        {
            var pizzaType = await _context.PizzaTypes
                .Include(pt => pt.Pizzas)
                .FirstOrDefaultAsync(pt => pt.PizzaTypeId == id);

            if (pizzaType == null)
                return NotFound();

            var totals = await _context.OrderDetails
                .GroupBy(od => od.PizzaId)
                .Select(g => new { PizzaId = g.Key, TotalSold = g.Sum(od => od.Quantity) })
                .ToDictionaryAsync(g => g.PizzaId, g => g.TotalSold);

            var dto = new PizzaTypeDto
            {
                PizzaTypeId = pizzaType.PizzaTypeId,
                Name = pizzaType.Name,
                Category = pizzaType.Category,
                Ingredients = pizzaType.Ingredients,
                Pizzas = pizzaType.Pizzas.Select(p => new PizzaDto
                {
                    PizzaId = p.PizzaId,
                    Size = p.Size,
                    Price = p.Price,
                    TotalSold = totals.TryGetValue(p.PizzaId, out var sold) ? sold : 0
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPizzaTypes(
        [FromQuery] string? name = null,
        [FromQuery] string? category = null,
        [FromQuery] string? ingredientContains = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var totals = await _context.OrderDetails
                .GroupBy(od => od.PizzaId)
                .Select(g => new { PizzaId = g.Key, TotalSold = g.Sum(od => od.Quantity) })
                .ToDictionaryAsync(g => g.PizzaId, g => g.TotalSold);

            var query = _context.PizzaTypes
                .Include(pt => pt.Pizzas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(pt => pt.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(pt => pt.Category == category);

            if (!string.IsNullOrWhiteSpace(ingredientContains))
                query = query.Where(pt => pt.Ingredients.Contains(ingredientContains));

            var pizzaTypes = await query
                .OrderBy(pt => pt.PizzaTypeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = pizzaTypes.Select(pt => new PizzaTypeDto
            {
                PizzaTypeId = pt.PizzaTypeId,
                Name = pt.Name,
                Category = pt.Category,
                Ingredients = pt.Ingredients,
                Pizzas = pt.Pizzas.Select(p => new PizzaDto
                {
                    PizzaId = p.PizzaId,
                    Size = p.Size,
                    Price = p.Price,
                    TotalSold = totals.TryGetValue(p.PizzaId, out var sold) ? sold : 0
                }).ToList()
            }).ToList();

            return Ok(result);
        }



        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var pizzaTypes = new List<PizzaType>();

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<PizzaTypeCsvMap>();
                try
                {
                    pizzaTypes = csv.GetRecords<PizzaType>().ToList();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Invalid CSV format: {ex.Message}");
                }
            }

            await _context.PizzaTypes.AddRangeAsync(pizzaTypes);
            await _context.SaveChangesAsync();

            return Ok(new { count = pizzaTypes.Count });
        }
    }

    public sealed class PizzaTypeCsvMap : CsvHelper.Configuration.ClassMap<PizzaType>
    {
        public PizzaTypeCsvMap()
        {
            Map(m => m.PizzaTypeId).Name("pizza_type_id");
            Map(m => m.Name).Name("name");
            Map(m => m.Category).Name("category");
            Map(m => m.Ingredients).Name("ingredients");
        }
    }
}
