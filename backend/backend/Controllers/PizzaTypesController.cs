using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;

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
        public async Task<IActionResult> PutPizzaType(int id, PizzaType pizzaType)
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

        private bool PizzaTypeExists(int id)
        {
            return _context.PizzaTypes.Any(e => e.PizzaTypeId == id);
        }
    }
}
