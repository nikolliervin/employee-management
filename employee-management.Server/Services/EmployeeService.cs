using AutoMapper;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Entities;

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

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllEmployeesAsync();

        var employees = await _employeeRepository.SearchAsync(searchTerm);
        return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto)
    {
        // Check if email already exists
        if (await _employeeRepository.EmailExistsAsync(createDto.Email))
            throw new InvalidOperationException($"Employee with email '{createDto.Email}' already exists.");

        var employee = _mapper.Map<Employee>(createDto);
        var createdEmployee = await _employeeRepository.AddAsync(employee);
        
        // Reload with department information for mapping
        var employeeWithDepartment = await _employeeRepository.GetByIdAsync(createdEmployee.Id);
        return _mapper.Map<EmployeeDto>(employeeWithDepartment);
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateDto)
    {
        // Check if employee exists
        if (!await _employeeRepository.ExistsAsync(id))
            throw new InvalidOperationException($"Employee with ID {id} not found.");

        // Check if email already exists for another employee
        if (await _employeeRepository.EmailExistsAsync(updateDto.Email, id))
            throw new InvalidOperationException($"Employee with email '{updateDto.Email}' already exists.");

        var employee = _mapper.Map<Employee>(updateDto);
        employee.Id = id;
        
        var updatedEmployee = await _employeeRepository.UpdateAsync(employee);
        
        // Reload with department information for mapping
        var employeeWithDepartment = await _employeeRepository.GetByIdAsync(id);
        return _mapper.Map<EmployeeDto>(employeeWithDepartment);
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        if (!await _employeeRepository.ExistsAsync(id))
            return false;

        return await _employeeRepository.DeleteAsync(id);
    }
} 