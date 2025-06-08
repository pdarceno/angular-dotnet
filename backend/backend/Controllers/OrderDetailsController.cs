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
    public class OrderDetailsController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public OrderDetailsController(PizzaDbContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetails
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(
            int page = 1, 
            int pageSize = 20, 
            string sortDir = "asc")
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Pizza)
                        .ThenInclude(p => p.PizzaType)
                .AsQueryable();

            // Apply sorting direction
            query = sortDir.ToLower() == "desc"
                ? query.OrderByDescending(o => o.OrderId)
                : query.OrderBy(o => o.OrderId);

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    Date = o.Date,
                    Time = o.Time,
                    Details = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        PizzaName = od.Pizza.PizzaType.Name,
                        Size = od.Pizza.Size,
                        Quantity = od.Quantity,
                        Price = od.Pizza.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }


        // GET: api/OrderDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Pizza)
                        .ThenInclude(p => p.PizzaType)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            var dto = new OrderDto
            {
                OrderId = order.OrderId,
                Date = order.Date,
                Time = order.Time,
                Details = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    OrderDetailId = od.OrderDetailId,
                    PizzaName = od.Pizza.PizzaType.Name,
                    Size = od.Pizza.Size,
                    Quantity = od.Quantity,
                    Price = od.Pizza.Price
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrderDetails(
        [FromQuery] DateOnly? date = null,
        [FromQuery] string? pizzaName = null,
        [FromQuery] string? size = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Pizza)
                        .ThenInclude(p => p.PizzaType)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(o => o.Date == date.Value);

            if (!string.IsNullOrWhiteSpace(pizzaName))
                query = query.Where(o =>
                    o.OrderDetails.Any(od =>
                        od.Pizza.PizzaType.Name.Contains(pizzaName)));

            if (!string.IsNullOrWhiteSpace(size))
                query = query.Where(o =>
                    o.OrderDetails.Any(od =>
                        od.Pizza.Size.Equals(size, StringComparison.OrdinalIgnoreCase)));

            var orders = await query
                .OrderBy(o => o.OrderId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    Date = o.Date,
                    Time = o.Time,
                    Details = o.OrderDetails.Select(od => new OrderDetailDto
                    {
                        OrderDetailId = od.OrderDetailId,
                        PizzaName = od.Pizza.PizzaType.Name,
                        Size = od.Pizza.Size,
                        Quantity = od.Quantity,
                        Price = od.Pizza.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            List<OrderDetail> orderDetails;
            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<OrderDetailCsvMap>();
                try
                {
                    orderDetails = csv.GetRecords<OrderDetail>().ToList();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Invalid CSV format: {ex.Message}");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Enable IDENTITY_INSERT on OrderDetails
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT OrderDetails ON");

                // Add manually specified identity values
                _context.OrderDetails.AddRange(orderDetails);
                await _context.SaveChangesAsync();

                // Turn it back off
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT OrderDetails OFF");

                // Optional: Reseed identity if necessary 
                // await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('OrderDetails', RESEED)");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }

            return Ok(new { count = orderDetails.Count });
        }
    }

    public sealed class OrderDetailCsvMap : ClassMap<OrderDetail>
    {
        public OrderDetailCsvMap()
        {
            Map(m => m.OrderDetailId).Name("order_details_id");
            Map(m => m.OrderId).Name("order_id");
            Map(m => m.PizzaId).Name("pizza_id");
            Map(m => m.Quantity).Name("quantity");
        }
    }
}
