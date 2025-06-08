using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public StatisticsController(PizzaDbContext context)
        {
            _context = context;
        }

        [HttpGet("bestsellers")]
        public async Task<IActionResult> GetBestsellers()
        {
            var top = await _context.OrderDetails
                .GroupBy(od => od.PizzaId)
                .Select(g => new {
                    PizzaId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToListAsync();

            return Ok(top);
        }

        [HttpGet("peak-hours")]
        public async Task<IActionResult> GetPeakHours()
        {
            var hourly = await _context.Orders
                .GroupBy(o => o.Time.Hour)
                .Select(g => new {
                    Hour = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .ToListAsync();

            return Ok(hourly);
        }

        [HttpGet("daily-orders")]
        public async Task<IActionResult> GetDailyOrders()
        {
            var daily = await _context.Orders
                .GroupBy(o => o.Date)
                .Select(g => new {
                    Date = g.Key,
                    Orders = g.Count()
                })
                .ToListAsync();

            return Ok(daily);
        }

        [HttpGet("underperformers")]
        public async Task<IActionResult> GetUnderperformers()
        {
            var orderedPizzaIds = await _context.OrderDetails
                .Select(od => od.PizzaId)
                .Distinct()
                .ToListAsync();

            var notOrdered = await _context.Pizzas
                .Where(p => !orderedPizzaIds.Contains(p.PizzaId))
                .Include(p => p.PizzaType)
                .ToListAsync();

            return Ok(notOrdered);
        }

    }
}
