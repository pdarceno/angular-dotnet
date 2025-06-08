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
using backend.DTOs;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly PizzaDbContext _context;

        public OrdersController(PizzaDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            List<Order> orders;
            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<OrderCsvMap>();
                try
                {
                    orders = csv.GetRecords<Order>().ToList();
                }
                catch (Exception ex)
                {
                    return BadRequest($"Invalid CSV format: {ex.Message}");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Enable IDENTITY_INSERT on Orders
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Orders ON");

                // Add manually specified identity values
                _context.Orders.AddRange(orders);
                await _context.SaveChangesAsync();

                // Turn it back off
                await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Orders OFF");

                // Optional: Reseed identity if necessary
                // await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Orders', RESEED)");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }

            return Ok(new { count = orders.Count });
        }
    }

    public sealed class OrderCsvMap : ClassMap<Order>
    {
        public OrderCsvMap()
        {
            Map(m => m.OrderId).Name("order_id");
            Map(m => m.Date).Name("date").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.Time).Name("time").TypeConverterOption.Format("HH:mm:ss");
        }
    }
}
