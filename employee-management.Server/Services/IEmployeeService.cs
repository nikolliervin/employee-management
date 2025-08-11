using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Services;

public interface IEmployeeService
{
    Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync();
    Task<ApiResponse<EmployeeDto>> GetEmployeeByIdAsync(Guid id);
    Task<ApiResponse<IEnumerable<EmployeeDto>>> SearchEmployeesAsync(string searchTerm);
    Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createDto);
    Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateDto);
    Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid id);
} 