using EmployeeApi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("bff/employees")]
public class EmployeeBFFController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EmployeeBFFController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("getall")]
    public async Task<IActionResult> GetAll()
    {
        var client = _httpClientFactory.CreateClient("EmployeeApi");
        var response = await client.GetAsync("api/employees");
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = content,
            StatusCode = (int)response.StatusCode,
            ContentType = "application/json"
        };
    }

    [HttpPost("getbyid")]
    public async Task<IActionResult> GetById([FromBody] int id)
    {
        var client = _httpClientFactory.CreateClient("EmployeeApi");
        var response = await client.GetAsync($"api/employees/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = content,
            StatusCode = (int)response.StatusCode,
            ContentType = "application/json"
        };
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(Employee employee)
    {
        var client = _httpClientFactory.CreateClient("EmployeeApi");
        var response = await client.PostAsJsonAsync("", employee);
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = content,
            StatusCode = (int)response.StatusCode,
            ContentType = "application/json"
        };
    }

    [HttpPost("delete")]
    public async Task<IActionResult> DeleteById([FromBody] int id)
    {
        var client = _httpClientFactory.CreateClient("EmployeeApi");
        var response = await client.DeleteAsync($"api/employees/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = content,
            StatusCode = (int)response.StatusCode,
            ContentType = "application/json"
        };
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateById([FromQuery] int id, [FromBody] Employee employee)
    {
        var client = _httpClientFactory.CreateClient("EmployeeApi");
        var response = await client.PutAsJsonAsync($"api/employees/{id}", employee);
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            Content = content,
            StatusCode = (int)response.StatusCode,
            ContentType = "application/json"
        };
    }
}
