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

    public async Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return ApiResponse<IEnumerable<EmployeeDto>>.Success(employeeDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<EmployeeDto>>.Error($"Failed to retrieve employees: {ex.Message}", 500);
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
            return ApiResponse<EmployeeDto>.Error($"Failed to retrieve employee: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<IEnumerable<EmployeeDto>>> SearchEmployeesAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return ApiResponse<IEnumerable<EmployeeDto>>.Error("Search term cannot be empty", 400);
            }

            var employees = await _employeeRepository.SearchAsync(searchTerm);
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return ApiResponse<IEnumerable<EmployeeDto>>.Success(employeeDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<EmployeeDto>>.Error($"Failed to search employees: {ex.Message}", 500);
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
            var createdEmployee = await _employeeRepository.AddAsync(employee);
            
            // Reload with department information for mapping
            var employeeWithDepartment = await _employeeRepository.GetByIdAsync(createdEmployee.Id);
            var employeeDto = _mapper.Map<EmployeeDto>(employeeWithDepartment);
            
            return ApiResponse<EmployeeDto>.Success(employeeDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto>.Error($"Failed to create employee: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateDto)
    {
        try
        {
            // Check if employee exists
            if (!await _employeeRepository.ExistsAsync(id))
            {
                return ApiResponse<EmployeeDto>.NotFound($"Employee with ID {id} not found");
            }

            // Check if email already exists for another employee
            if (await _employeeRepository.EmailExistsAsync(updateDto.Email, id))
            {
                return ApiResponse<EmployeeDto>.Conflict($"Employee with email '{updateDto.Email}' already exists");
            }

            var employee = _mapper.Map<Employee>(updateDto);
            employee.Id = id;
            
            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);
            
            // Reload with department information for mapping
            var employeeWithDepartment = await _employeeRepository.GetByIdAsync(id);
            var employeeDto = _mapper.Map<EmployeeDto>(employeeWithDepartment);
            
            return ApiResponse<EmployeeDto>.Success(employeeDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto>.Error($"Failed to update employee: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid id)
    {
        try
        {
            if (!await _employeeRepository.ExistsAsync(id))
            {
                return ApiResponse<bool>.NotFound($"Employee with ID {id} not found");
            }

            var deleted = await _employeeRepository.DeleteAsync(id);
            if (deleted)
            {
                return ApiResponse<bool>.Success(true);
            }
            else
            {
                return ApiResponse<bool>.Error("Failed to delete employee", 500);
            }
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Error($"Failed to delete employee: {ex.Message}", 500);
        }
    }
} 