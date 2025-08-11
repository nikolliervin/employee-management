using Microsoft.AspNetCore.Mvc;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using employee_management.Server.Services;

namespace employee_management.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetEmployees()
    {
        var response = await _employeeService.GetAllEmployeesAsync();
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/employees/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(Guid id)
    {
        var response = await _employeeService.GetEmployeeByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/employees/search?term={searchTerm}
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> SearchEmployees([FromQuery] string term)
    {
        var response = await _employeeService.SearchEmployeesAsync(term);
        return StatusCode(response.StatusCode, response);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> CreateEmployee(CreateEmployeeDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            var validationResponse = ApiResponse<EmployeeDto>.ValidationError(errors);
            return StatusCode(validationResponse.StatusCode, validationResponse);
        }

        var response = await _employeeService.CreateEmployeeAsync(createDto);
        return StatusCode(response.StatusCode, response);
    }

    // PUT: api/employees/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> UpdateEmployee(Guid id, UpdateEmployeeDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            var validationResponse = ApiResponse<EmployeeDto>.ValidationError(errors);
            return StatusCode(validationResponse.StatusCode, validationResponse);
        }

        var response = await _employeeService.UpdateEmployeeAsync(id, updateDto);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE: api/employees/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(Guid id)
    {
        var response = await _employeeService.DeleteEmployeeAsync(id);
        return StatusCode(response.StatusCode, response);
    }
} 