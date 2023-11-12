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
    public class CustomerController : ControllerBase
    {
        private readonly NorthwindContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(NorthwindContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers.ToListAsync();
                return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(string id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving customer with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer([FromBody] Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating the customer");
                return BadRequest("Error creating the customer");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(string id, [FromBody] Customer customer)
        {
            try
            {
                if (id != customer.CustomerId)
                {
                    return BadRequest();
                }

                _context.Entry(customer).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error updating customer with ID: {id}");
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error updating customer with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);

                if (customer == null)
                {
                    return NotFound();
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting customer with ID: {id}");
                return BadRequest("Error deleting the customer");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error deleting customer with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
