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
    public class EmployeeController : ControllerBase
    {
        private readonly NorthwindContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(NorthwindContext context, ILogger<EmployeeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return NotFound();
                }

                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving employee with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee([FromBody] Employee employee)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating the employee");
                return BadRequest("Error creating the employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Employee employee)
        {
            try
            {
                if (id != employee.EmployeeId)
                {
                    return BadRequest();
                }

                _context.Entry(employee).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error updating employee with ID: {id}");
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error updating employee with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return NotFound();
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID: {id}");
                return BadRequest("Error deleting the employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error deleting employee with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
