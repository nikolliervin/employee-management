using Microsoft.AspNetCore.Mvc;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using employee_management.Server.Services;

namespace employee_management.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: api/v1/employees
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<EmployeeDto>>>> GetEmployees([FromQuery] PaginationRequest pagination)
    {
        var response = await _employeeService.GetAllEmployeesAsync(pagination);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/v1/employees/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(Guid id)
    {
        var response = await _employeeService.GetEmployeeByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/v1/employees/search
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<EmployeeDto>>>> SearchEmployees([FromQuery] SearchRequest request)
    {
        var response = await _employeeService.SearchEmployeesAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/v1/employees/deleted
    [HttpGet("deleted")]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetDeletedEmployees()
    {
        var response = await _employeeService.GetDeletedEmployeesAsync();
        return StatusCode(response.StatusCode, response);
    }

    // POST: api/v1/employees
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

    // PUT: api/v1/employees/{id}
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

    // DELETE: api/v1/employees/{id} (Soft Delete)
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(Guid id)
    {
        var response = await _employeeService.DeleteEmployeeAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    // POST: api/v1/employees/{id}/restore
    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult<ApiResponse<bool>>> RestoreEmployee(Guid id)
    {
        var response = await _employeeService.RestoreEmployeeAsync(id);
        return StatusCode(response.StatusCode, response);
    }
} 