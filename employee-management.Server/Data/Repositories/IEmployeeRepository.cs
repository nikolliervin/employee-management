using employee_management.Server.Models.Entities;

namespace employee_management.Server.Data.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(Guid id);
    Task<Employee> AddAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task<IEnumerable<Employee>> SearchAsync(string searchTerm);
    Task<IEnumerable<Employee>> GetDeletedAsync();
    Task<bool> RestoreAsync(Guid id);
} 