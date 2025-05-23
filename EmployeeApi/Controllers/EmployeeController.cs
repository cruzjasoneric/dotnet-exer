using EmployeeApi.Data;
using EmployeeApi.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(EmployeeDbContext context, IValidator<Employee> createEmployeeValidator, IValidator<PatchEmployee> updateEmployeeValidator, ILogger<EmployeesController> logger) : ControllerBase
    {
        private readonly EmployeeDbContext _context = context;
        private readonly IValidator<Employee> _createEmployeeValidator = createEmployeeValidator;
        private readonly IValidator<PatchEmployee> _updateEmployeeValidator = updateEmployeeValidator;
        private readonly ILogger<EmployeesController> _logger = logger;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
        {
            //throw new Exception("test exception");
            try
            {
                var employees = await _context.Employees.ToListAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching employee data");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
            

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> Get(int id)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(id);
                if (emp == null) return NotFound();
                return Ok(emp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching employee data");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> Create(Employee employee)
        {
            try
            {
                // validation
                var validationResult = await _createEmployeeValidator.ValidateAsync(employee);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                        .ToList();

                    if (errors.Count > 0)
                    {
                        _logger.LogError("Trying to create employee record but failed due to validation error(s)");
                    }
                    foreach (var error in validationResult.Errors)
                    {
                        _logger.LogWarning("Validation failed for {Property}: {ErrorMessage}", error.PropertyName, error.ErrorMessage);
                    }

                    return BadRequest("Invalid fields provided");
                }

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = employee.Id }, employee);
            }
            catch (DbException dbEx)
            {
                _logger.LogError(dbEx, "Database update failed while creating employee.");
                return StatusCode(500, "Database update failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating employee data");
                return StatusCode(500, "Internal server error. Please try again later.");
            }

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, PatchEmployee employee)
        {
            try
            {
                // validation
                var validationResult = await _updateEmployeeValidator.ValidateAsync(employee);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed for PATCH employee");
                    foreach (var error in validationResult.Errors)
                    {
                        _logger.LogWarning("Validation failed for {Property}: {ErrorMessage}", error.PropertyName, error.ErrorMessage);
                    }

                    return BadRequest("Invalid fields provided.");
                }

                var employeeToUpdate = await _context.Employees.FindAsync(id);
                if (employeeToUpdate == null)
                {
                    return NotFound();
                }

                if (employee.FirstName != null) employeeToUpdate.FirstName = employee.FirstName;
                if (employee.LastName != null) employeeToUpdate.LastName = employee.LastName;
                if (employee.Email != null) employeeToUpdate.Email = employee.Email;
                if (employee.Phone != null) employeeToUpdate.Phone = employee.Phone;
                if (employee.Department != null) employeeToUpdate.Department = employee.Department;
                if (employee.HireDate.HasValue) employeeToUpdate.HireDate = employee.HireDate.Value;
                if (employee.Salary != null) employeeToUpdate.Salary = employee.Salary.Value;

                await _context.SaveChangesAsync();
                return Ok(employeeToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while patching employee.");
                return StatusCode(500, "Something went wrong.");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(id);
                if (emp == null) return NotFound();

                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();

                _logger.LogWarning($"Employee id={id} has been deleted.");
                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting employee data");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}
