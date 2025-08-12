using AutoMapper;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;

    public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedResult<DepartmentDto>>> GetAllDepartmentsAsync(PaginationRequest request)
    {
        try
        {
            var paginatedDepartments = await _departmentRepository.GetAllAsync(request);
            var departmentDtos = _mapper.Map<IEnumerable<DepartmentDto>>(paginatedDepartments.Data);
            
            var paginatedResult = new PaginatedResult<DepartmentDto>(
                departmentDtos, 
                paginatedDepartments.TotalCount, 
                paginatedDepartments.PageNumber, 
                paginatedDepartments.PageSize
            );
            
            return ApiResponse<PaginatedResult<DepartmentDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResult<DepartmentDto>>.Error($"Failed to retrieve departments: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PaginatedResult<DepartmentDto>>> SearchDepartmentsAsync(SearchRequest request)
    {
        try
        {
            // Validate that at least one search criteria is provided
            if (string.IsNullOrWhiteSpace(request.SearchTerm) && 
                string.IsNullOrWhiteSpace(request.Name) && 
                request.CreatedAtFrom == null &&
                request.CreatedAtTo == null)
            {
                return ApiResponse<PaginatedResult<DepartmentDto>>.ValidationError(new List<string> { "At least one search criteria must be provided" });
            }

            var paginatedDepartments = await _departmentRepository.SearchAsync(request);
            var departmentDtos = _mapper.Map<IEnumerable<DepartmentDto>>(paginatedDepartments.Data);
            
            var paginatedResult = new PaginatedResult<DepartmentDto>(
                departmentDtos, 
                paginatedDepartments.TotalCount, 
                paginatedDepartments.PageNumber, 
                paginatedDepartments.PageSize
            );
            
            return ApiResponse<PaginatedResult<DepartmentDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResult<DepartmentDto>>.Error($"Failed to search departments: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(Guid id)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                return ApiResponse<DepartmentDto>.NotFound("Department not found");
            }

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return ApiResponse<DepartmentDto>.Success(departmentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<DepartmentDto>.Error($"Failed to retrieve department: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto createDto)
    {
        try
        {
            // Check if department name already exists
            if (await _departmentRepository.NameExistsAsync(createDto.Name))
            {
                return ApiResponse<DepartmentDto>.ValidationError(new List<string> { "Department name already exists" });
            }

            var department = _mapper.Map<Department>(createDto);
            var createdDepartment = await _departmentRepository.CreateAsync(department);
            var departmentDto = _mapper.Map<DepartmentDto>(createdDepartment);
            
            return ApiResponse<DepartmentDto>.Success(departmentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<DepartmentDto>.Error($"Failed to create department: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto updateDto)
    {
        try
        {
            var existingDepartment = await _departmentRepository.GetByIdAsync(id);
            if (existingDepartment == null)
            {
                return ApiResponse<DepartmentDto>.NotFound("Department not found");
            }

            // Check if department name already exists (excluding current department)
            if (await _departmentRepository.NameExistsAsync(updateDto.Name, id))
            {
                return ApiResponse<DepartmentDto>.ValidationError(new List<string> { "Department name already exists" });
            }

            _mapper.Map(updateDto, existingDepartment);
            var updatedDepartment = await _departmentRepository.UpdateAsync(existingDepartment);
            var departmentDto = _mapper.Map<DepartmentDto>(updatedDepartment);
            
            return ApiResponse<DepartmentDto>.Success(departmentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<DepartmentDto>.Error($"Failed to update department: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteDepartmentAsync(Guid id)
    {
        try
        {
            var result = await _departmentRepository.DeleteAsync(id);
            if (!result)
            {
                return ApiResponse<bool>.NotFound("Department not found");
            }
            
            return ApiResponse<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<bool>.ValidationError(new List<string> { ex.Message });
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"Failed to delete department: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> RestoreDepartmentAsync(Guid id)
    {
        try
        {
            var result = await _departmentRepository.RestoreAsync(id);
            if (!result)
            {
                return ApiResponse<bool>.NotFound("Department not found or not deleted");
            }
            
            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"Failed to restore department: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<DepartmentDto>>> GetDeletedDepartmentsAsync()
    {
        try
        {
            var deletedDepartments = await _departmentRepository.GetDeletedAsync();
            var departmentDtos = _mapper.Map<IEnumerable<DepartmentDto>>(deletedDepartments);
            
            return ApiResponse<IEnumerable<DepartmentDto>>.Success(departmentDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<DepartmentDto>>.Error($"Failed to retrieve deleted departments: {ex.Message}");
        }
    }
}