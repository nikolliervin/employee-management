using employee_management.Server.Models.DTOs;

namespace employee_management.Server.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
    Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto);
    Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateDto);
    Task<bool> DeleteEmployeeAsync(int id);
} 