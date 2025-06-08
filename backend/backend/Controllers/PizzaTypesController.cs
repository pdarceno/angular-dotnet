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
        public async Task<ActionResult<PizzaType>> GetPizzaType(int id)
        {
            var pizzaType = await _context.PizzaTypes.FindAsync(id);

            if (pizzaType == null)
            {
                return NotFound();
            }

            return pizzaType;
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
