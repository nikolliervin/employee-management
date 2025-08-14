using employee_management.Server.Models.Entities;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Data.Repositories;

/// <summary>
/// Repository interface for department data access operations.
/// Defines the contract for all department-related database operations including CRUD operations,
/// search, pagination, and soft delete/restore functionality.
/// </summary>
/// <example>
/// // Dependency injection usage
/// services.AddScoped&lt;IDepartmentRepository, DepartmentRepository&gt;();
/// 
/// // In a service
/// public class DepartmentService : IDepartmentService
/// {
///     private readonly IDepartmentRepository _departmentRepository;
///     
///     public DepartmentService(IDepartmentRepository departmentRepository)
///     {
///         _departmentRepository = departmentRepository;
///     }
/// }
/// </example>
public interface IDepartmentRepository
{
    /// <summary>
    /// Retrieves a paginated list of all active departments.
    /// </summary>
    /// <param name="pagination">Pagination parameters including page number, page size, and sorting options.</param>
    /// <returns>A paginated result containing departments and metadata.</returns>
    /// <example>
    /// var pagination = new PaginationRequest { PageNumber = 1, PageSize = 10 };
    /// var result = await _departmentRepository.GetAllAsync(pagination);
    /// </example>
    Task<PaginatedResult<Department>> GetAllAsync(PaginationRequest pagination);
    
    /// <summary>
    /// Retrieves a department by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the department to retrieve.</param>
    /// <returns>The department if found, null otherwise.</returns>
    /// <example>
    /// var department = await _departmentRepository.GetByIdAsync(Guid.Parse("456e7890-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<Department?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Creates a new department in the database.
    /// </summary>
    /// <param name="department">The department entity to create.</param>
    /// <returns>The created department with generated ID and audit fields populated.</returns>
    /// <example>
    /// var newDepartment = new Department { Name = "Engineering", Description = "Software development team" };
    /// var createdDepartment = await _departmentRepository.CreateAsync(newDepartment);
    /// </example>
    Task<Department> CreateAsync(Department department);
    
    /// <summary>
    /// Updates an existing department in the database.
    /// </summary>
    /// <param name="department">The department entity with updated values.</param>
    /// <returns>The updated department with audit fields updated.</returns>
    /// <example>
    /// department.Name = "Engineering & Development";
    /// var updatedDepartment = await _departmentRepository.UpdateAsync(department);
    /// </example>
    Task<Department> UpdateAsync(Department department);
    
    /// <summary>
    /// Soft deletes a department by marking it as deleted without removing from database.
    /// Note: Departments with active employees cannot be deleted.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the department to delete.</param>
    /// <returns>True if the department was successfully soft-deleted, false otherwise.</returns>
    /// <example>
    /// var deleted = await _departmentRepository.DeleteAsync(Guid.Parse("456e7890-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Restores a previously soft-deleted department.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the department to restore.</param>
    /// <returns>True if the department was successfully restored, false otherwise.</returns>
    /// <example>
    /// var restored = await _departmentRepository.RestoreAsync(Guid.Parse("456e7890-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<bool> RestoreAsync(Guid id);
    
    /// <summary>
    /// Retrieves all soft-deleted departments for potential restoration.
    /// </summary>
    /// <returns>A collection of all soft-deleted departments.</returns>
    /// <example>
    /// var deletedDepartments = await _departmentRepository.GetDeletedAsync();
    /// foreach (var department in deletedDepartments)
    /// {
    ///     Console.WriteLine($"Deleted: {department.Name} on {department.DeletedAt}");
    /// }
    /// </example>
    Task<IEnumerable<Department>> GetDeletedAsync();
    
    /// <summary>
    /// Searches for departments based on specified criteria with pagination support.
    /// </summary>
    /// <param name="request">Search criteria including search term, filters, and pagination parameters.</param>
    /// <returns>A paginated result containing matching departments and metadata.</returns>
    /// <example>
    /// var searchRequest = new SearchRequest 
    /// { 
    ///     SearchTerm = "engineering", 
    ///     PageNumber = 1, 
    ///     PageSize = 10
    /// };
    /// var result = await _departmentRepository.SearchAsync(searchRequest);
    /// </example>
    Task<PaginatedResult<Department>> SearchAsync(SearchRequest request);
    
    /// <summary>
    /// Checks if a department with the specified ID exists in the database.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) to check.</param>
    /// <returns>True if the department exists, false otherwise.</returns>
    /// <example>
    /// var exists = await _departmentRepository.ExistsAsync(Guid.Parse("456e7890-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<bool> ExistsAsync(Guid id);
    
    /// <summary>
    /// Checks if a department with the specified name exists.
    /// Optionally excludes a specific department ID from the check (useful for updates).
    /// </summary>
    /// <param name="name">The department name to check for uniqueness.</param>
    /// <param name="excludeId">Optional department ID to exclude from the uniqueness check.</param>
    /// <returns>True if the name exists, false otherwise.</returns>
    /// <example>
    /// // Check if name exists (for new departments)
    /// var nameExists = await _departmentRepository.NameExistsAsync("Engineering");
    /// 
    /// // Check if name exists excluding current department (for updates)
    /// var nameExists = await _departmentRepository.NameExistsAsync("Engineering", currentDepartmentId);
    /// </example>
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
}