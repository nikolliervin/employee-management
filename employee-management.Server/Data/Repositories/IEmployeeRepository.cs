using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Data.Repositories;

public interface IEmployeeRepository
{
    // Paginated methods
    Task<PaginatedResult<Employee>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = "Name", string sortOrder = "asc");
    Task<PaginatedResult<Employee>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10, string? sortBy = "Name", string sortOrder = "asc");
    
    // Other methods
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee> AddAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<IEnumerable<Employee>> GetDeletedAsync();
    Task<bool> RestoreAsync(Guid id);
} 