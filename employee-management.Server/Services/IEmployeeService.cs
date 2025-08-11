using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Services;

public interface IEmployeeService
{
    // Updated to return paginated results
    Task<ApiResponse<PaginatedResult<EmployeeDto>>> GetAllEmployeesAsync(PaginationRequest request);
    Task<ApiResponse<PaginatedResult<EmployeeDto>>> SearchEmployeesAsync(SearchRequest request);
    
    // Other methods remain the same
    Task<ApiResponse<EmployeeDto>> GetEmployeeByIdAsync(Guid id);
    Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createDto);
    Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateDto);
    Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid id);
    Task<ApiResponse<IEnumerable<EmployeeDto>>> GetDeletedEmployeesAsync();
    Task<ApiResponse<bool>> RestoreEmployeeAsync(Guid id);
} 