using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Services;

/// <summary>
/// Contract for employee service operations including CRUD, search, soft delete, and restore functionality.
/// Provides business logic layer between controllers and data access layer.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Retrieves a paginated list of all active employees with optional sorting.
    /// </summary>
    /// <param name="request">Pagination request containing page number, page size, and sorting parameters</param>
    /// <returns>API response containing paginated employee data with total count and page information</returns>
    Task<ApiResponse<PaginatedResult<EmployeeDto>>> GetAllEmployeesAsync(PaginationRequest request);
    
    /// <summary>
    /// Searches employees based on multiple criteria with pagination support.
    /// </summary>
    /// <param name="request">Search request containing search terms, filters, and pagination parameters</param>
    /// <returns>API response containing paginated search results matching the specified criteria</returns>
    Task<ApiResponse<PaginatedResult<EmployeeDto>>> SearchEmployeesAsync(SearchRequest request);
    
    /// <summary>
    /// Retrieves a specific employee by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to retrieve</param>
    /// <returns>API response containing the employee data or not found result</returns>
    Task<ApiResponse<EmployeeDto>> GetEmployeeByIdAsync(Guid id);
    
    /// <summary>
    /// Creates a new employee record with validation and email uniqueness check.
    /// </summary>
    /// <param name="createDto">Data transfer object containing employee information for creation</param>
    /// <returns>API response containing the created employee data or validation errors</returns>
    Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createDto);
    
    /// <summary>
    /// Updates an existing employee record with validation and email uniqueness check.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update</param>
    /// <param name="updateDto">Data transfer object containing updated employee information</param>
    /// <returns>API response containing the updated employee data or validation errors</returns>
    Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateDto);
    
    /// <summary>
    /// Performs soft delete on an employee record, marking it as deleted without removing from database.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete</param>
    /// <returns>API response indicating success or failure of the delete operation</returns>
    Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid id);
    
    /// <summary>
    /// Retrieves all employees that have been soft deleted from the system.
    /// </summary>
    /// <returns>API response containing a list of deleted employee records</returns>
    Task<ApiResponse<IEnumerable<EmployeeDto>>> GetDeletedEmployeesAsync();
    
    /// <summary>
    /// Restores a previously soft-deleted employee record, making it active again.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to restore</param>
    /// <returns>API response indicating success or failure of the restore operation</returns>
    Task<ApiResponse<bool>> RestoreEmployeeAsync(Guid id);
} 