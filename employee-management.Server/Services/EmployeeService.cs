using AutoMapper;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;
using employee_management.Server.Constants;

namespace employee_management.Server.Services;

/// <summary>
/// Service implementation for employee operations including CRUD, search, and soft delete functionality.
/// Handles business logic, validation, and coordinates between controllers and data repositories.
/// Uses AutoMapper for entity-DTO conversions and implements comprehensive error handling with logging.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeeService> _logger;

    /// <summary>
    /// Initializes a new instance of the EmployeeService with required dependencies.
    /// </summary>
    /// <param name="employeeRepository">Repository for employee data access operations</param>
    /// <param name="mapper">AutoMapper instance for entity-DTO mapping</param>
    /// <param name="logger">Logger instance for error and information logging</param>
    public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, ILogger<EmployeeService> logger)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of all active employees with optional sorting.
    /// Maps entity results to DTOs and handles any exceptions that occur during data retrieval.
    /// </summary>
    /// <param name="request">Pagination request containing page number, page size, and sorting parameters</param>
    /// <returns>API response containing paginated employee data with total count and page information</returns>
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
            _logger.LogError(ex, "Failed to retrieve employees with pagination request: {@PaginationRequest}", request);
            return ApiResponse<PaginatedResult<EmployeeDto>>.Error(ErrorMessages.Employee.RetrievalFailed, 500);
        }
    }

    /// <summary>
    /// Searches employees based on multiple criteria with pagination support.
    /// Validates that at least one search criterion is provided before executing the search.
    /// Supports searching by name, email, department, and date ranges.
    /// </summary>
    /// <param name="request">Search request containing search terms, filters, and pagination parameters</param>
    /// <returns>API response containing paginated search results matching the specified criteria</returns>
    public async Task<ApiResponse<PaginatedResult<EmployeeDto>>> SearchEmployeesAsync(SearchRequest request)
    {
        try
        {
            // Validate that at least one search criteria is provided
            if (string.IsNullOrWhiteSpace(request.SearchTerm) && 
                request.DepartmentId == null && 
                string.IsNullOrWhiteSpace(request.Name) && 
                string.IsNullOrWhiteSpace(request.Email) &&
                request.DateOfBirthFrom == null &&
                request.DateOfBirthTo == null &&
                request.CreatedAtFrom == null &&
                request.CreatedAtTo == null)
            {
                return ApiResponse<PaginatedResult<EmployeeDto>>.ValidationError(new List<string> { ErrorMessages.Employee.SearchCriteriaRequired });
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
            _logger.LogError(ex, "Failed to search employees with request: {@SearchRequest}", request);
            return ApiResponse<PaginatedResult<EmployeeDto>>.Error(ErrorMessages.Employee.SearchFailed, 500);
        }
    }

    /// <summary>
    /// Retrieves a specific employee by their unique identifier.
    /// Returns a not found response if the employee does not exist or has been soft deleted.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to retrieve</param>
    /// <returns>API response containing the employee data or not found result</returns>
    public async Task<ApiResponse<EmployeeDto>> GetEmployeeByIdAsync(Guid id)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return ApiResponse<EmployeeDto>.NotFound(string.Format(ErrorMessages.Employee.NotFound, id));

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return ApiResponse<EmployeeDto>.Success(employeeDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve employee with ID: {EmployeeId}", id);
            return ApiResponse<EmployeeDto>.Error(ErrorMessages.Employee.RetrievalFailed, 500);
        }
    }

    /// <summary>
    /// Creates a new employee record with validation and email uniqueness check.
    /// Verifies that the email address is not already in use before creating the employee.
    /// </summary>
    /// <param name="createDto">Data transfer object containing employee information for creation</param>
    /// <returns>API response containing the created employee data or validation/conflict errors</returns>
    public async Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto createDto)
    {
        try
        {
            // Check if email already exists
            if (await _employeeRepository.EmailExistsAsync(createDto.Email))
                return ApiResponse<EmployeeDto>.Conflict(string.Format(ErrorMessages.Employee.EmailAlreadyExists, createDto.Email));

            var employee = _mapper.Map<Employee>(createDto);
            var result = await _employeeRepository.AddAsync(employee);
            var employeeDto = _mapper.Map<EmployeeDto>(result);
            
            return ApiResponse<EmployeeDto>.Created(employeeDto, ErrorMessages.Employee.CreatedSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create employee with data: {@CreateEmployeeDto}", createDto);
            return ApiResponse<EmployeeDto>.Error(ErrorMessages.Employee.CreationFailed, 500);
        }
    }

    /// <summary>
    /// Updates an existing employee record with validation and email uniqueness check.
    /// Verifies the employee exists and that the new email is not already in use by another employee.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to update</param>
    /// <param name="updateDto">Data transfer object containing updated employee information</param>
    /// <returns>API response containing the updated employee data or validation/conflict errors</returns>
    public async Task<ApiResponse<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto updateDto)
    {
        try
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(id);
            if (existingEmployee == null)
                return ApiResponse<EmployeeDto>.NotFound(string.Format(ErrorMessages.Employee.NotFound, id));

            // Check if email already exists (excluding current employee)
            if (await _employeeRepository.EmailExistsAsync(updateDto.Email, id))
                return ApiResponse<EmployeeDto>.Conflict(string.Format(ErrorMessages.Employee.EmailAlreadyExists, updateDto.Email));

            _mapper.Map(updateDto, existingEmployee);
            var result = await _employeeRepository.UpdateAsync(existingEmployee);
            var employeeDto = _mapper.Map<EmployeeDto>(result);
            
            return ApiResponse<EmployeeDto>.Success(employeeDto, ErrorMessages.Employee.UpdatedSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update employee with ID: {EmployeeId}, Data: {@UpdateEmployeeDto}", id, updateDto);
            return ApiResponse<EmployeeDto>.Error(ErrorMessages.Employee.UpdateFailed, 500);
        }
    }

    /// <summary>
    /// Performs soft delete on an employee record, marking it as deleted without removing from database.
    /// The employee will be excluded from normal queries but can be restored later if needed.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to delete</param>
    /// <returns>API response indicating success or failure of the delete operation</returns>
    public async Task<ApiResponse<bool>> DeleteEmployeeAsync(Guid id)
    {
        try
        {
            var deleted = await _employeeRepository.DeleteAsync(id);
            if (deleted)
                return ApiResponse<bool>.Success(true, ErrorMessages.Employee.DeletedSuccessfully);
            else
                return ApiResponse<bool>.NotFound(string.Format(ErrorMessages.Employee.NotFound, id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete employee with ID: {EmployeeId}", id);
            return ApiResponse<bool>.Error(ErrorMessages.Employee.DeletionFailed, 500);
        }
    }

    /// <summary>
    /// Retrieves all employees that have been soft deleted from the system.
    /// These records are excluded from normal queries but retained for audit purposes and potential restoration.
    /// </summary>
    /// <returns>API response containing a list of deleted employee records</returns>
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
            _logger.LogError(ex, "Failed to retrieve deleted employees");
            return ApiResponse<IEnumerable<EmployeeDto>>.Error(ErrorMessages.Employee.RetrievalFailed, 500);
        }
    }

    /// <summary>
    /// Restores a previously soft-deleted employee record, making it active again.
    /// Clears the deletion markers and makes the employee visible in normal queries.
    /// </summary>
    /// <param name="id">The unique identifier of the employee to restore</param>
    /// <returns>API response indicating success or failure of the restore operation</returns>
    public async Task<ApiResponse<bool>> RestoreEmployeeAsync(Guid id)
    {
        try
        {
            var restored = await _employeeRepository.RestoreAsync(id);
            if (restored)
                return ApiResponse<bool>.Success(true, ErrorMessages.Employee.RestoredSuccessfully);
            else
                return ApiResponse<bool>.NotFound(string.Format(ErrorMessages.Employee.NotFoundOrNotDeleted, id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore employee with ID: {EmployeeId}", id);
            return ApiResponse<bool>.Error(ErrorMessages.Employee.RestoreFailed, 500);
        }
    }
} 