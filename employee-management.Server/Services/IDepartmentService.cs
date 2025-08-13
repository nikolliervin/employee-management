using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Services;

/// <summary>
/// Contract for department service operations including CRUD, search, soft delete, and restore functionality.
/// Provides business logic layer for department management with employee count tracking.
/// </summary>
public interface IDepartmentService
{
    /// <summary>
    /// Retrieves a paginated list of all active departments with optional sorting.
    /// </summary>
    /// <param name="request">Pagination request containing page number, page size, and sorting parameters</param>
    /// <returns>API response containing paginated department data with employee counts</returns>
    Task<ApiResponse<PaginatedResult<DepartmentDto>>> GetAllDepartmentsAsync(PaginationRequest request);
    
    /// <summary>
    /// Retrieves a specific department by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the department to retrieve</param>
    /// <returns>API response containing the department data or not found result</returns>
    Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(Guid id);
    
    /// <summary>
    /// Creates a new department record with validation.
    /// </summary>
    /// <param name="createDto">Data transfer object containing department information for creation</param>
    /// <returns>API response containing the created department data or validation errors</returns>
    Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto createDto);
    
    /// <summary>
    /// Updates an existing department record with validation.
    /// </summary>
    /// <param name="id">The unique identifier of the department to update</param>
    /// <param name="updateDto">Data transfer object containing updated department information</param>
    /// <returns>API response containing the updated department data or validation errors</returns>
    Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto updateDto);
    
    /// <summary>
    /// Performs soft delete on a department record, marking it as deleted without removing from database.
    /// </summary>
    /// <param name="id">The unique identifier of the department to delete</param>
    /// <returns>API response indicating success or failure of the delete operation</returns>
    Task<ApiResponse<bool>> DeleteDepartmentAsync(Guid id);
    
    /// <summary>
    /// Restores a previously soft-deleted department record, making it active again.
    /// </summary>
    /// <param name="id">The unique identifier of the department to restore</param>
    /// <returns>API response indicating success or failure of the restore operation</returns>
    Task<ApiResponse<bool>> RestoreDepartmentAsync(Guid id);
    
    /// <summary>
    /// Retrieves all departments that have been soft deleted from the system.
    /// </summary>
    /// <returns>API response containing a list of deleted department records</returns>
    Task<ApiResponse<IEnumerable<DepartmentDto>>> GetDeletedDepartmentsAsync();
    
    /// <summary>
    /// Searches departments based on specified criteria with pagination support.
    /// </summary>
    /// <param name="request">Search request containing search terms, filters, and pagination parameters</param>
    /// <returns>API response containing paginated search results matching the specified criteria</returns>
    Task<ApiResponse<PaginatedResult<DepartmentDto>>> SearchDepartmentsAsync(SearchRequest request);
}