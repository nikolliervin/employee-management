using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Services;

public interface IDepartmentService
{
    Task<ApiResponse<PaginatedResult<DepartmentDto>>> GetAllDepartmentsAsync(PaginationRequest request);
    Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(Guid id);
    Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto createDto);
    Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto updateDto);
    Task<ApiResponse<bool>> DeleteDepartmentAsync(Guid id);
    Task<ApiResponse<bool>> RestoreDepartmentAsync(Guid id);
    Task<ApiResponse<IEnumerable<DepartmentDto>>> GetDeletedDepartmentsAsync();
    Task<ApiResponse<PaginatedResult<DepartmentDto>>> SearchDepartmentsAsync(SearchRequest request);
}