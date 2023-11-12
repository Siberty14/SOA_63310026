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
    public class OrderDetailController : ControllerBase
    {
        private readonly NorthwindContext _context;
        private readonly ILogger<OrderDetailController> _logger;

        public OrderDetailController(NorthwindContext context, ILogger<OrderDetailController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orderdetail>>> GetOrderDetails()
        {
            try
            {
                var orderDetails = await _context.Orderdetails.ToListAsync();
                return orderDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Orderdetail>> GetOrderDetail(int id)
        {
            try
            {
                var orderDetail = await _context.Orderdetails.FindAsync(id);

                if (orderDetail == null)
                {
                    return NotFound();
                }

                return orderDetail;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order detail with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Orderdetail>> PostOrderDetail([FromBody] Orderdetail orderDetail)
        {
            try
            {
                _context.Orderdetails.Add(orderDetail);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.OrderDetailsId }, orderDetail);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating the order detail");
                return BadRequest("Error creating the order detail");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, [FromBody] Orderdetail orderDetail)
        {
            try
            {
                if (id != orderDetail.OrderDetailsId)
                {
                    return BadRequest();
                }

                _context.Entry(orderDetail).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error updating order detail with ID: {id}");
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error updating order detail with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            try
            {
                var orderDetail = await _context.Orderdetails.FindAsync(id);

                if (orderDetail == null)
                {
                    return NotFound();
                }

                _context.Orderdetails.Remove(orderDetail);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting order detail with ID: {id}");
                return BadRequest("Error deleting the order detail");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error deleting order detail with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool OrderDetailExists(int id)
        {
            return _context.Orderdetails.Any(e => e.OrderDetailsId == id);
        }
    }
}
