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
    public class SupplierController : ControllerBase
    {
        private readonly NorthwindContext _context;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(NorthwindContext context, ILogger<SupplierController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            try
            {
                var suppliers = await _context.Suppliers.ToListAsync();
                return suppliers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving suppliers");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            try
            {
                var supplier = await _context.Suppliers.FindAsync(id);

                if (supplier == null)
                {
                    return NotFound();
                }

                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving supplier with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> PostSupplier([FromBody] Supplier supplier)
        {
            try
            {
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSupplier), new { id = supplier.SupplierId }, supplier);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating the supplier");
                return BadRequest("Error creating the supplier");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplier(int id, [FromBody] Supplier supplier)
        {
            try
            {
                if (id != supplier.SupplierId)
                {
                    return BadRequest();
                }

                _context.Entry(supplier).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error updating supplier with ID: {id}");
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error updating supplier with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                var supplier = await _context.Suppliers.FindAsync(id);

                if (supplier == null)
                {
                    return NotFound();
                }

                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting supplier with ID: {id}");
                return BadRequest("Error deleting the supplier");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error deleting supplier with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
