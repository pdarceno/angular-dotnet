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
        public async Task<ActionResult<IEnumerable<PizzaType>>> GetPizzaTypes()
        {
            return await _context.PizzaTypes.ToListAsync();
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

        // PUT: api/PizzaTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPizzaType(string id, PizzaType pizzaType)
        {
            if (id != pizzaType.PizzaTypeId)
            {
                return BadRequest();
            }

            _context.Entry(pizzaType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PizzaTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PizzaTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PizzaType>> PostPizzaType(PizzaType pizzaType)
        {
            _context.PizzaTypes.Add(pizzaType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPizzaType", new { id = pizzaType.PizzaTypeId }, pizzaType);
        }

        // DELETE: api/PizzaTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePizzaType(int id)
        {
            var pizzaType = await _context.PizzaTypes.FindAsync(id);
            if (pizzaType == null)
            {
                return NotFound();
            }

            _context.PizzaTypes.Remove(pizzaType);
            await _context.SaveChangesAsync();

            return NoContent();
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

        private bool PizzaTypeExists(string id)
        {
            return _context.PizzaTypes.Any(e => e.PizzaTypeId == id);
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
