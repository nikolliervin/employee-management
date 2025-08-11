using AutoMapper;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedResult<EmployeeDto>>> GetAllEmployeesAsync(PaginationRequest request)
    {
        try
        {
            var paginatedEmployees = await _employeeRepository.GetAllAsync(request);
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(paginatedEmployees.Data);
            
            var paginatedResult = new PaginatedResult<EmployeeDto>(
                employeeDtos, 
                paginatedEmployees.TotalCount, 
                paginatedEmployees.PageNumber, 
                paginatedEmployees.PageSize
            );
            
            return ApiResponse<PaginatedResult<EmployeeDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResult<EmployeeDto>>.Error($"Failed to retrieve employees: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PaginatedResult<EmployeeDto>>> SearchEmployeesAsync(SearchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return ApiResponse<PaginatedResult<EmployeeDto>>.ValidationError(new List<string> { "Search term cannot be empty" });
            }

            var paginatedEmployees = await _employeeRepository.SearchAsync(request);
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(paginatedEmployees.Data);
            
            var paginatedResult = new PaginatedResult<EmployeeDto>(
                employeeDtos, 
                paginatedEmployees.TotalCount, 
                paginatedEmployees.PageNumber, 
                paginatedEmployees.PageSize
            );
            
            return ApiResponse<PaginatedResult<EmployeeDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaginatedResult<EmployeeDto>>.Error($"Failed to search employees: {ex.Message}");
        }
    }

    public async Task<ApiResponse<EmployeeDto>> GetEmployeeByIdAsync(Guid id)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                return ApiResponse<EmployeeDto>.NotFound($"Employee with ID {id} not found");
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return ApiResponse<EmployeeDto>.Success(employeeDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto>.Error($"Failed to retrieve employee: {ex.Message}");
        }
    }

    public async Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createDto)
    {
        try
        {
            // Check if email already exists
            if (await _employeeRepository.EmailExistsAsync(createDto.Email))
            {
                return ApiResponse<EmployeeDto>.Conflict($"Employee with email '{createDto.Email}' already exists");
            }

            var employee = _mapper.Map<Employee>(createDto);
            var result = await _employeeRepository.AddAsync(employee);
            var employeeDto = _mapper.Map<EmployeeDto>(result);
            
            return ApiResponse<EmployeeDto>.Success(employeeDto, "Employee created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto>.Error($"Failed to create employee: {ex.Message}");
        }
    }

    public async Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateDto)
    {
        try
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return ApiResponse<EmployeeDto>.NotFound($"Employee with ID {id} not found");
            }

            // Check if email already exists (excluding current employee)
            if (await _employeeRepository.EmailExistsAsync(updateDto.Email, id))
            {
                return ApiResponse<EmployeeDto>.Conflict($"Employee with email '{updateDto.Email}' already exists");
            }

            _mapper.Map(updateDto, existingEmployee);
            var result = await _employeeRepository.UpdateAsync(existingEmployee);
            var employeeDto = _mapper.Map<EmployeeDto>(result);
            
            return ApiResponse<EmployeeDto>.Success(employeeDto, "Employee updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto>.Error($"Failed to update employee: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid id)
    {
        try
        {
            var deleted = await _employeeRepository.DeleteAsync(id);
            if (deleted)
            {
                return ApiResponse<bool>.Success(true, "Employee soft deleted successfully");
            }
            else
            {
                return ApiResponse<bool>.NotFound($"Employee with ID {id} not found");
            }
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"Failed to delete employee: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<EmployeeDto>>> GetDeletedEmployeesAsync()
    {
        try
        {
            var deletedEmployees = await _employeeRepository.GetDeletedAsync();
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(deletedEmployees);
            return ApiResponse<IEnumerable<EmployeeDto>>.Success(employeeDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<EmployeeDto>>.Error($"Failed to retrieve deleted employees: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> RestoreEmployeeAsync(Guid id)
    {
        try
        {
            var restored = await _employeeRepository.RestoreAsync(id);
            if (restored)
            {
                return ApiResponse<bool>.Success(true, "Employee restored successfully");
            }
            else
            {
                return ApiResponse<bool>.NotFound($"Employee with ID {id} not found or not deleted");
            }
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"Failed to restore employee: {ex.Message}");
        }
    }
} 