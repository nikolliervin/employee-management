using Microsoft.AspNetCore.Mvc;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using employee_management.Server.Services;
using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Controllers;

/// <summary>
/// Manages employee operations including CRUD operations, search, and soft delete/restore functionality.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiResponse<object>), 400)]
[ProducesResponseType(typeof(ApiResponse<object>), 500)]
[Tags("Employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Retrieves a paginated list of all employees.
    /// </summary>
    /// <param name="pagination">Pagination parameters including page number and page size.</param>
    /// <returns>A paginated list of employees with metadata.</returns>
    /// <response code="200">Successfully retrieved the list of employees.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/employees?pageNumber=1&pageSize=10
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "items": [...],
    ///     "totalCount": 50,
    ///     "pageNumber": 1,
    ///     "pageSize": 10,
    ///     "totalPages": 5
    ///   }
    /// }
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<EmployeeDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<EmployeeDto>>>> GetEmployees([FromQuery] PaginationRequest pagination)
    {
        var response = await _employeeService.GetAllEmployeesAsync(pagination);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Retrieves a specific employee by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employee.</param>
    /// <returns>The employee details if found.</returns>
    /// <response code="200">Successfully retrieved the employee.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="400">Invalid employee ID format.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/employees/123e4567-e89b-12d3-a456-426614174000
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "id": "123e4567-e89b-12d3-a456-426614174000",
    ///     "name": "John Doe",
    ///     "email": "john.doe@example.com",
    ///     "dateOfBirth": "1990-01-01",
    ///     "departmentId": "456e7890-e89b-12d3-a456-426614174000",
    ///     "departmentName": "Engineering"
    ///   }
    /// }
    /// </example>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(Guid id)
    {
        var response = await _employeeService.GetEmployeeByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Searches for employees based on specified criteria with pagination support.
    /// </summary>
    /// <param name="request">Search criteria including search term, filters, and pagination.</param>
    /// <returns>A paginated list of employees matching the search criteria.</returns>
    /// <response code="200">Successfully retrieved search results.</response>
    /// <response code="400">Invalid search parameters.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/employees/search?searchTerm=john&pageNumber=1&pageSize=10
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "items": [...],
    ///     "totalCount": 5,
    ///     "pageNumber": 1,
    ///     "pageSize": 10,
    ///     "totalPages": 1
    ///   }
    /// }
    /// </example>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<EmployeeDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<EmployeeDto>>>> SearchEmployees([FromQuery] SearchRequest request)
    {
        var response = await _employeeService.SearchEmployeesAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Retrieves all soft-deleted employees for potential restoration.
    /// </summary>
    /// <returns>A list of all soft-deleted employees.</returns>
    /// <response code="200">Successfully retrieved deleted employees.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/employees/deleted
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "id": "123e4567-e89b-12d3-a456-426614174000",
    ///       "name": "John Doe",
    ///       "email": "john.doe@example.com",
    ///       "deletedAt": "2024-01-15T10:30:00Z"
    ///     }
    ///   ]
    /// }
    /// </example>
    [HttpGet("deleted")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDto>>>> GetDeletedEmployees()
    {
        var response = await _employeeService.GetDeletedEmployeesAsync();
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Creates a new employee with the provided information.
    /// </summary>
    /// <param name="createDto">The employee creation data including name, email, date of birth, and department.</param>
    /// <returns>The newly created employee with generated ID and timestamps.</returns>
    /// <response code="201">Successfully created the employee.</response>
    /// <response code="400">Validation errors or invalid data.</response>
    /// <response code="409">Employee with the same email already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: POST /api/v1/employees
    /// Body: 
    /// {
    ///   "name": "Jane Smith",
    ///   "email": "jane.smith@example.com",
    ///   "dateOfBirth": "1992-05-15",
    ///   "departmentId": "456e7890-e89b-12d3-a456-426614174000"
    /// }
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "id": "789e0123-e89b-12d3-a456-426614174000",
    ///     "name": "Jane Smith",
    ///     "email": "jane.smith@example.com",
    ///     "dateOfBirth": "1992-05-15",
    ///     "departmentId": "456e7890-e89b-12d3-a456-426614174000",
    ///     "createdAt": "2024-01-20T14:30:00Z"
    ///   }
    /// }
    /// </example>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> CreateEmployee([FromBody] CreateEmployeeDto createDto)
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

    /// <summary>
    /// Updates an existing employee's information.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employee to update.</param>
    /// <param name="updateDto">The updated employee information.</param>
    /// <returns>The updated employee details.</returns>
    /// <response code="200">Successfully updated the employee.</response>
    /// <response code="400">Validation errors or invalid data.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="409">Employee with the same email already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: PUT /api/v1/employees/123e4567-e89b-12d3-a456-426614174000
    /// Body: 
    /// {
    ///   "name": "John Doe Updated",
    ///   "email": "john.doe.updated@example.com",
    ///   "dateOfBirth": "1990-01-01",
    ///   "departmentId": "456e7890-e89b-12d3-a456-426614174000"
    /// }
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "id": "123e4567-e89b-12d3-a456-426614174000",
    ///     "name": "John Doe Updated",
    ///     "email": "john.doe.updated@example.com",
    ///     "updatedAt": "2024-01-20T15:45:00Z"
    ///   }
    /// }
    /// </example>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeDto updateDto)
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

    /// <summary>
    /// Soft deletes an employee (marks as deleted without removing from database).
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employee to delete.</param>
    /// <returns>True if the employee was successfully soft-deleted.</returns>
    /// <response code="200">Successfully soft-deleted the employee.</response>
    /// <response code="404">Employee not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: DELETE /api/v1/employees/123e4567-e89b-12d3-a456-426614174000
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": true,
    ///   "message": "Employee deleted successfully"
    /// }
    /// </example>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(Guid id)
    {
        var response = await _employeeService.DeleteEmployeeAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Restores a previously soft-deleted employee.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employee to restore.</param>
    /// <returns>True if the employee was successfully restored.</returns>
    /// <response code="200">Successfully restored the employee.</response>
    /// <response code="404">Employee not found or not deleted.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: POST /api/v1/employees/123e4567-e89b-12d3-a456-426614174000/restore
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": true,
    ///   "message": "Employee restored successfully"
    /// }
    /// </example>
    [HttpPost("{id:guid}/restore")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> RestoreEmployee(Guid id)
    {
        var response = await _employeeService.RestoreEmployeeAsync(id);
        return StatusCode(response.StatusCode, response);
    }
} 