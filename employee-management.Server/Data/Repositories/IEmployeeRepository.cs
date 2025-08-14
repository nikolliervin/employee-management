using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;
using employee_management.Server.Models.DTOs;

namespace employee_management.Server.Data.Repositories;

/// <summary>
/// Repository interface for employee data access operations.
/// Defines the contract for all employee-related database operations including CRUD operations,
/// search, pagination, and soft delete/restore functionality.
/// </summary>
/// <example>
/// // Dependency injection usage
/// services.AddScoped&lt;IEmployeeRepository, EmployeeRepository&gt;();
/// 
/// // In a service
/// public class EmployeeService : IEmployeeService
/// {
///     private readonly IEmployeeRepository _employeeRepository;
///     
///     public EmployeeService(IEmployeeRepository employeeRepository)
///     {
///         _employeeRepository = employeeRepository;
///     }
/// }
/// </example>
public interface IEmployeeRepository
{
    /// <summary>
    /// Retrieves a paginated list of all active employees.
    /// </summary>
    /// <param name="request">Pagination parameters including page number, page size, and sorting options.</param>
    /// <returns>A paginated result containing employees and metadata.</returns>
    /// <example>
    /// var pagination = new PaginationRequest { PageNumber = 1, PageSize = 10 };
    /// var result = await _employeeRepository.GetAllAsync(pagination);
    /// </example>
    Task<PaginatedResult<Employee>> GetAllAsync(PaginationRequest request);
    
    /// <summary>
    /// Searches for employees based on specified criteria with pagination support.
    /// </summary>
    /// <param name="request">Search criteria including search term, filters, and pagination parameters.</param>
    /// <returns>A paginated result containing matching employees and metadata.</returns>
    /// <example>
    /// var searchRequest = new SearchRequest 
    /// { 
    ///     SearchTerm = "john", 
    ///     PageNumber = 1, 
    ///     PageSize = 10,
    ///     DepartmentId = departmentId
    /// };
    /// var result = await _employeeRepository.SearchAsync(searchRequest);
    /// </example>
    Task<PaginatedResult<Employee>> SearchAsync(SearchRequest request);
    
    /// <summary>
    /// Retrieves an employee by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employee to retrieve.</param>
    /// <returns>The employee if found, null otherwise.</returns>
    /// <example>
    /// var employee = await _employeeRepository.GetByIdAsync(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<Employee?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Adds a new employee to the database.
    /// </summary>
    /// <param name="employee">The employee entity to add.</param>
    /// <returns>The added employee with generated ID and audit fields populated.</returns>
    /// <example>
    /// var newEmployee = new Employee { Name = "John Doe", Email = "john@example.com" };
    /// var addedEmployee = await _employeeRepository.AddAsync(newEmployee);
    /// </example>
    Task<Employee> AddAsync(Employee employee);
    
    /// <summary>
    /// Updates an existing employee in the database.
    /// </summary>
    /// <param name="employee">The employee entity with updated values.</param>
    /// <returns>The updated employee with audit fields updated.</returns>
    /// <example>
    /// employee.Name = "John Doe Updated";
    /// var updatedEmployee = await _employeeRepository.UpdateAsync(employee);
    /// </example>
    Task<Employee> UpdateAsync(Employee employee);
    
    /// <summary>
    /// Soft deletes an employee by marking it as deleted without removing from database.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employee to delete.</param>
    /// <returns>True if the employee was successfully soft-deleted, false otherwise.</returns>
    /// <example>
    /// var deleted = await _employeeRepository.DeleteAsync(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<bool> DeleteAsync(Guid id);
    
    /// <summary>
    /// Checks if an employee with the specified ID exists in the database.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) to check.</param>
    /// <returns>True if the employee exists, false otherwise.</returns>
    /// <example>
    /// var exists = await _employeeRepository.ExistsAsync(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<bool> ExistsAsync(Guid id);
    
    /// <summary>
    /// Checks if an employee with the specified email address exists.
    /// Optionally excludes a specific employee ID from the check (useful for updates).
    /// </summary>
    /// <param name="email">The email address to check for uniqueness.</param>
    /// <param name="excludeId">Optional employee ID to exclude from the uniqueness check.</param>
    /// <returns>True if the email exists, false otherwise.</returns>
    /// <example>
    /// // Check if email exists (for new employees)
    /// var emailExists = await _employeeRepository.EmailExistsAsync("john@example.com");
    /// 
    /// // Check if email exists excluding current employee (for updates)
    /// var emailExists = await _employeeRepository.EmailExistsAsync("john@example.com", currentEmployeeId);
    /// </example>
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    
    /// <summary>
    /// Retrieves all soft-deleted employees for potential restoration.
    /// </summary>
    /// <returns>A collection of all soft-deleted employees.</returns>
    /// <example>
    /// var deletedEmployees = await _employeeRepository.GetDeletedAsync();
    /// foreach (var employee in deletedEmployees)
    /// {
    ///     Console.WriteLine($"Deleted: {employee.Name} on {employee.DeletedAt}");
    /// }
    /// </example>
    Task<IEnumerable<Employee>> GetDeletedAsync();
    
    /// <summary>
    /// Restores a previously soft-deleted employee.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the employee to restore.</param>
    /// <returns>True if the employee was successfully restored, false otherwise.</returns>
    /// <example>
    /// var restored = await _employeeRepository.RestoreAsync(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"));
    /// </example>
    Task<bool> RestoreAsync(Guid id);
} 