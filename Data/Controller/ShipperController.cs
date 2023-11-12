using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Data; // Ensure you have the correct namespace for your DbContext
using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShipperController : ControllerBase
    {
        private readonly NorthwindContext _context;
        private readonly ILogger<ShipperController> _logger;

        public ShipperController(NorthwindContext context, ILogger<ShipperController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shipper>>> GetShippers()
        {
            try
            {
                var shippers = await _context.Shippers.ToListAsync();
                return shippers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving shippers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Shipper>> GetShipper(int id)
        {
            try
            {
                var shipper = await _context.Shippers.FindAsync(id);

                if (shipper == null)
                {
                    return NotFound();
                }

                return shipper;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving shipper with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Shipper>> PostShipper([FromBody] Shipper shipper)
        {
            try
            {
                _context.Shippers.Add(shipper);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetShipper), new { id = shipper.ShipperId }, shipper);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating the shipper");
                return BadRequest("Error creating the shipper");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutShipper(int id, [FromBody] Shipper shipper)
        {
            try
            {
                if (id != shipper.ShipperId)
                {
                    return BadRequest();
                }

                _context.Entry(shipper).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShipperExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error updating shipper with ID: {id}");
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error updating shipper with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipper(int id)
        {
            try
            {
                var shipper = await _context.Shippers.FindAsync(id);

                if (shipper == null)
                {
                    return NotFound();
                }

                _context.Shippers.Remove(shipper);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting shipper with ID: {id}");
                return BadRequest("Error deleting the shipper");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error deleting shipper with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool ShipperExists(int id)
        {
            return _context.Shippers.Any(e => e.ShipperId == id);
        }
    }
}
