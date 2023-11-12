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
    public class OrderController : ControllerBase
    {
        private readonly NorthwindContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(NorthwindContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                var orders = await _context.Orders.ToListAsync();
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);

                if (order == null)
                {
                    return NotFound();
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder([FromBody] Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating the order");
                return BadRequest("Error creating the order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, [FromBody] Order order)
        {
            try
            {
                if (id != order.OrderId)
                {
                    return BadRequest();
                }

                _context.Entry(order).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error updating order with ID: {id}");
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error updating order with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);

                if (order == null)
                {
                    return NotFound();
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting order with ID: {id}");
                return BadRequest("Error deleting the order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error deleting order with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
