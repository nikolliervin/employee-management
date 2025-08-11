using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;
using employee_management.Server.Models.DTOs;

namespace employee_management.Server.Data.Repositories;

public interface IEmployeeRepository
{
    // Paginated methods
    Task<PaginatedResult<Employee>> GetAllAsync(PaginationRequest request);
    Task<PaginatedResult<Employee>> SearchAsync(SearchRequest request);
    
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