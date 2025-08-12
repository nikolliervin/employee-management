using employee_management.Server.Models.Entities;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Data.Repositories;

public interface IDepartmentRepository
{
    Task<PaginatedResult<Department>> GetAllAsync(PaginationRequest pagination);
    Task<Department?> GetByIdAsync(Guid id);
    Task<Department> CreateAsync(Department department);
    Task<Department> UpdateAsync(Department department);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> RestoreAsync(Guid id);
    Task<IEnumerable<Department>> GetDeletedAsync();
    Task<PaginatedResult<Department>> SearchAsync(SearchRequest request);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null);
}