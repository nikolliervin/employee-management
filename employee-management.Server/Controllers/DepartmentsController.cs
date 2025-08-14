using Microsoft.AspNetCore.Mvc;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using employee_management.Server.Services;
using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Controllers;

/// <summary>
/// Manages department operations including CRUD operations, search, and soft delete/restore functionality.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ApiResponse<object>), 400)]
[ProducesResponseType(typeof(ApiResponse<object>), 500)]
[Tags("Departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    /// <summary>
    /// Retrieves a paginated list of all departments.
    /// </summary>
    /// <param name="pagination">Pagination parameters including page number and page size.</param>
    /// <returns>A paginated list of departments with metadata.</returns>
    /// <response code="200">Successfully retrieved the list of departments.</response>
    /// <response code="400">Invalid pagination parameters.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/departments?pageNumber=1&pageSize=10
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "items": [
    ///       {
    ///         "id": "456e7890-e89b-12d3-a456-426614174000",
    ///         "name": "Engineering",
    ///         "description": "Software development and engineering team",
    ///         "employeeCount": 25
    ///       }
    ///     ],
    ///     "totalCount": 8,
    ///     "pageNumber": 1,
    ///     "pageSize": 10,
    ///     "totalPages": 1
    ///   }
    /// }
    /// </example>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<DepartmentDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<DepartmentDto>>>> GetDepartments([FromQuery] PaginationRequest pagination)
    {
        var response = await _departmentService.GetAllDepartmentsAsync(pagination);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Retrieves a specific department by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the department.</param>
    /// <returns>The department details if found.</returns>
    /// <response code="200">Successfully retrieved the department.</response>
    /// <response code="404">Department not found.</response>
    /// <response code="400">Invalid department ID format.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/departments/456e7890-e89b-12d3-a456-426614174000
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "id": "456e7890-e89b-12d3-a456-426614174000",
    ///     "name": "Engineering",
    ///     "description": "Software development and engineering team",
    ///     "employeeCount": 25,
    ///     "createdAt": "2024-01-01T00:00:00Z"
    ///   }
    /// }
    /// </example>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartment(Guid id)
    {
        var response = await _departmentService.GetDepartmentByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Searches for departments based on specified criteria with pagination support.
    /// </summary>
    /// <param name="request">Search criteria including search term, filters, and pagination.</param>
    /// <returns>A paginated list of departments matching the search criteria.</returns>
    /// <response code="200">Successfully retrieved search results.</response>
    /// <response code="400">Invalid search parameters.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/departments/search?searchTerm=engineering&pageNumber=1&pageSize=10
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "items": [
    ///       {
    ///         "id": "456e7890-e89b-12d3-a456-426614174000",
    ///         "name": "Engineering",
    ///         "description": "Software development and engineering team"
    ///       }
    ///     ],
    ///     "totalCount": 1,
    ///     "pageNumber": 1,
    ///     "pageSize": 10,
    ///     "totalPages": 1
    ///   }
    /// }
    /// </example>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<DepartmentDto>>), 200)]
    public async Task<ActionResult<ApiResponse<PaginatedResult<DepartmentDto>>>> SearchDepartments([FromQuery] SearchRequest request)
    {
        var response = await _departmentService.SearchDepartmentsAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Retrieves all soft-deleted departments for potential restoration.
    /// </summary>
    /// <returns>A list of all soft-deleted departments.</returns>
    /// <response code="200">Successfully retrieved deleted departments.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: GET /api/v1/departments/deleted
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "id": "456e7890-e89b-12d3-a456-426614174000",
    ///       "name": "Old Department",
    ///       "description": "Department that was deleted",
    ///       "deletedAt": "2024-01-15T10:30:00Z"
    ///     }
    ///   ]
    /// }
    /// </example>
    [HttpGet("deleted")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentDto>>), 200)]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetDeletedDepartments()
    {
        var response = await _departmentService.GetDeletedDepartmentsAsync();
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Creates a new department with the provided information.
    /// </summary>
    /// <param name="createDto">The department creation data including name and description.</param>
    /// <returns>The newly created department with generated ID and timestamps.</returns>
    /// <response code="201">Successfully created the department.</response>
    /// <response code="400">Validation errors or invalid data.</response>
    /// <response code="409">Department with the same name already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: POST /api/v1/departments
    /// Body: 
    /// {
    ///   "name": "Marketing",
    ///   "description": "Digital marketing and brand management team"
    /// }
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "id": "789e0123-e89b-12d3-a456-426614174000",
    ///     "name": "Marketing",
    ///     "description": "Digital marketing and brand management team",
    ///     "createdAt": "2024-01-20T14:30:00Z"
    ///   }
    /// }
    /// </example>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> CreateDepartment([FromBody] CreateDepartmentDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            var validationResponse = ApiResponse<DepartmentDto>.ValidationError(errors);
            return StatusCode(validationResponse.StatusCode, validationResponse);
        }

        var response = await _departmentService.CreateDepartmentAsync(createDto);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Updates an existing department's information.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the department to update.</param>
    /// <param name="updateDto">The updated department information.</param>
    /// <returns>The updated department details.</returns>
    /// <response code="200">Successfully updated the department.</response>
    /// <response code="400">Validation errors or invalid data.</response>
    /// <response code="404">Department not found.</response>
    /// <response code="409">Department with the same name already exists.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: PUT /api/v1/departments/456e7890-e89b-12d3-a456-426614174000
    /// Body: 
    /// {
    ///   "name": "Engineering & Development",
    ///   "description": "Updated description for engineering team"
    /// }
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "id": "456e7890-e89b-12d3-a456-426614174000",
    ///     "name": "Engineering & Development",
    ///     "description": "Updated description for engineering team",
    ///     "updatedAt": "2024-01-20T15:45:00Z"
    ///   }
    /// }
    /// </example>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 409)]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            var validationResponse = ApiResponse<DepartmentDto>.ValidationError(errors);
            return StatusCode(validationResponse.StatusCode, validationResponse);
        }

        var response = await _departmentService.UpdateDepartmentAsync(id, updateDto);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Soft deletes a department (marks as deleted without removing from database).
    /// Note: Departments with active employees cannot be deleted.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the department to delete.</param>
    /// <returns>True if the department was successfully soft-deleted.</returns>
    /// <response code="200">Successfully soft-deleted the department.</response>
    /// <response code="400">Department has active employees and cannot be deleted.</response>
    /// <response code="404">Department not found.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: DELETE /api/v1/departments/456e7890-e89b-12d3-a456-426614174000
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": true,
    ///   "message": "Department deleted successfully"
    /// }
    /// </example>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteDepartment(Guid id)
    {
        var response = await _departmentService.DeleteDepartmentAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Restores a previously soft-deleted department.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the department to restore.</param>
    /// <returns>True if the department was successfully restored.</returns>
    /// <response code="200">Successfully restored the department.</response>
    /// <response code="404">Department not found or not deleted.</response>
    /// <response code="500">Internal server error occurred.</response>
    /// <example>
    /// Request: POST /api/v1/departments/456e7890-e89b-12d3-a456-426614174000/restore
    /// Response: 
    /// {
    ///   "success": true,
    ///   "data": true,
    ///   "message": "Department restored successfully"
    /// }
    /// </example>
    [HttpPost("{id:guid}/restore")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<bool>>> RestoreDepartment(Guid id)
    {
        var response = await _departmentService.RestoreDepartmentAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}