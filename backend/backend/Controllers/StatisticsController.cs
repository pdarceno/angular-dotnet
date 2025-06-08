using backend.DTOs;
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
                .Select(g => new
                {
                    PizzaId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .Join(
                    _context.Pizzas.Include(p => p.PizzaType),
                    best => best.PizzaId,
                    pizza => pizza.PizzaId,
                    (best, pizza) => new PizzaDto
                    {
                        PizzaId = pizza.PizzaId,
                        PizzaTypeName = pizza.PizzaType.Name,
                        Category = pizza.PizzaType.Category,
                        Size = pizza.Size,
                        Price = pizza.Price,
                        TotalSold = best.TotalSold
                    }
                )
                .ToListAsync();

            return Ok(top);
        }

        [HttpGet("peak-hours")]
        public async Task<IActionResult> GetPeakHours()
        {
            var hourly = await _context.Orders
                .GroupBy(o => o.DateTime.Hour)
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
                .GroupBy(o => o.DateTime.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Orders = g.Count()
                })
                .OrderByDescending(x => x.Date)
                .Take(10)
                .OrderBy(x => x.Date) // re-sort for chart display
                .ToListAsync();

            return Ok(daily);
        }



        [HttpGet("underperformers")]
        public async Task<IActionResult> GetUnderperformers()
        {
            var notOrdered = await _context.Pizzas
                .Where(p => !_context.OrderDetails.Any(od => od.PizzaId == p.PizzaId))
                .Include(p => p.PizzaType)
                .ToListAsync();

            var result = notOrdered.Select(p => new PizzaDto
            {
                PizzaId = p.PizzaId,
                PizzaTypeName = p.PizzaType.Name,
                Category = p.PizzaType.Category,
                Size = p.Size,
                Price = p.Price
            }).ToList();

            return Ok(result);
        }
    }
}
