using EmployeeApi.Models;
using EmployeeBFF.Models.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeBFF.Controllers
{
    [ApiController]
    [Route("bff/employees")]
    public class EmployeeBFFController(ILogger<EmployeeBFFController> logger, IValidator<EmployeeCreateRequest> createEmployeeValidator, IValidator<EmployeeUpdateRequest> updateEmployeeValidator, IHttpClientFactory httpClientFactory) : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IValidator<EmployeeCreateRequest> _createEmployeeValidator = createEmployeeValidator;
        private readonly IValidator<EmployeeUpdateRequest> _updateEmployeeValidator = updateEmployeeValidator;
        private readonly ILogger<EmployeeBFFController> _logger = logger;


        [HttpPost("getall")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("EmployeeApi");
                var response = await client.GetAsync("api/employees");
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Fetch successful");
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching from API");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost("getbyid")]
        public async Task<IActionResult> GetById([FromBody] int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("EmployeeApi");
                var response = await client.GetAsync($"api/employees/{id}");
                var content = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching from API");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BffEmployeeCreateRequest request)
        {
            try
            {
                var employee = request.Data;

                var validationResult = await _createEmployeeValidator.ValidateAsync(employee);
                if (!validationResult.IsValid)
                {
                    return BadRequest("There were invalid field(s) in create request");
                }

                var client = _httpClientFactory.CreateClient("EmployeeApi");
                var response = await client.PostAsJsonAsync($"api/employees", employee);
                var content = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, content);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employee");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteById([FromBody] int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("EmployeeApi");
                var response = await client.DeleteAsync($"api/employees/{id}");
                var content = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {id}");
                return StatusCode(500, new { Error = "Failed to delete employee" });
            }


        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateById([FromBody] BffEmployeePatchRequest request)
        {
            var employeeId = request.Id;
            var employee = request.Data;
            try
            {
                var validationResult = await _updateEmployeeValidator.ValidateAsync(employee);
                if (!validationResult.IsValid)
                {
                    return BadRequest("There were invalid field(s) in update request");
                }

                var client = _httpClientFactory.CreateClient("EmployeeApi");
                var response = await client.PatchAsJsonAsync($"api/employees/{employeeId}", employee);
                var content = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error patching employee with ID {employeeId}");
                return StatusCode(500, new { Error = "Failed to patch employee" });
            }
        }
    }
}