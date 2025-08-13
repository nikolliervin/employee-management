using AutoMapper;
using employee_management.Server.Data.Repositories;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;
using employee_management.Server.Constants;

namespace employee_management.Server.Services;

/// <summary>
/// Service implementation for department operations including CRUD, search, and soft delete functionality.
/// Handles business logic for department management and coordinates between controllers and data repositories.
/// Uses AutoMapper for entity-DTO conversions and implements comprehensive error handling with logging.
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DepartmentService> _logger;

    /// <summary>
    /// Initializes a new instance of the DepartmentService with required dependencies.
    /// </summary>
    /// <param name="departmentRepository">Repository for department data access operations</param>
    /// <param name="mapper">AutoMapper instance for entity-DTO mapping</param>
    /// <param name="logger">Logger instance for error and information logging</param>
    public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper, ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of all active departments with optional sorting.
    /// Maps entity results to DTOs and handles any exceptions that occur during data retrieval.
    /// </summary>
    /// <param name="request">Pagination request containing page number, page size, and sorting parameters</param>
    /// <returns>API response containing paginated department data with total count and page information</returns>
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
            _logger.LogError(ex, "Failed to retrieve departments with pagination request: {@PaginationRequest}", request);
            return ApiResponse<PaginatedResult<DepartmentDto>>.Error(ErrorMessages.Department.RetrievalFailed, 500);
        }
    }

    /// <summary>
    /// Searches departments based on specified criteria with pagination support.
    /// Validates that at least one search criterion is provided before executing the search.
    /// Supports searching by name and date ranges.
    /// </summary>
    /// <param name="request">Search request containing search terms, filters, and pagination parameters</param>
    /// <returns>API response containing paginated search results matching the specified criteria</returns>
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
                return ApiResponse<PaginatedResult<DepartmentDto>>.ValidationError(new List<string> { ErrorMessages.Department.SearchCriteriaRequired });
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
            _logger.LogError(ex, "Failed to search departments with request: {@SearchRequest}", request);
            return ApiResponse<PaginatedResult<DepartmentDto>>.Error(ErrorMessages.Department.SearchFailed, 500);
        }
    }

    /// <summary>
    /// Retrieves a specific department by its unique identifier.
    /// Returns a not found response if the department does not exist or has been soft deleted.
    /// </summary>
    /// <param name="id">The unique identifier of the department to retrieve</param>
    /// <returns>API response containing the department data or not found result</returns>
    public async Task<ApiResponse<DepartmentDto>> GetDepartmentByIdAsync(Guid id)
    {
        try
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
                return ApiResponse<DepartmentDto>.NotFound(ErrorMessages.Department.NotFoundGeneric);

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return ApiResponse<DepartmentDto>.Success(departmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve department with ID: {DepartmentId}", id);
            return ApiResponse<DepartmentDto>.Error(ErrorMessages.Department.RetrievalFailed, 500);
        }
    }

    /// <summary>
    /// Creates a new department record with validation and name uniqueness check.
    /// Verifies that the department name is not already in use before creating the department.
    /// </summary>
    /// <param name="createDto">Data transfer object containing department information for creation</param>
    /// <returns>API response containing the created department data or validation/conflict errors</returns>
    public async Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto createDto)
    {
        try
        {
            // Check if department name already exists
            if (await _departmentRepository.NameExistsAsync(createDto.Name))
                return ApiResponse<DepartmentDto>.ValidationError(new List<string> { ErrorMessages.Department.NameAlreadyExists });

            var department = _mapper.Map<Department>(createDto);
            var createdDepartment = await _departmentRepository.CreateAsync(department);
            var departmentDto = _mapper.Map<DepartmentDto>(createdDepartment);
            
            return ApiResponse<DepartmentDto>.Created(departmentDto, ErrorMessages.Department.CreatedSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create department with data: {@CreateDepartmentDto}", createDto);
            return ApiResponse<DepartmentDto>.Error(ErrorMessages.Department.CreationFailed, 500);
        }
    }

    /// <summary>
    /// Updates an existing department record with validation and name uniqueness check.
    /// Verifies the department exists and that the new name is not already in use by another department.
    /// </summary>
    /// <param name="id">The unique identifier of the department to update</param>
    /// <param name="updateDto">Data transfer object containing updated department information</param>
    /// <returns>API response containing the updated department data or validation/conflict errors</returns>
    public async Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto updateDto)
    {
        try
        {
            var existingDepartment = await _departmentRepository.GetByIdAsync(id);
            if (existingDepartment == null)
                return ApiResponse<DepartmentDto>.NotFound(ErrorMessages.Department.NotFoundGeneric);

            // Check if department name already exists (excluding current department)
            if (await _departmentRepository.NameExistsAsync(updateDto.Name, id))
                return ApiResponse<DepartmentDto>.ValidationError(new List<string> { ErrorMessages.Department.NameAlreadyExists });

            _mapper.Map(updateDto, existingDepartment);
            var updatedDepartment = await _departmentRepository.UpdateAsync(existingDepartment);
            var departmentDto = _mapper.Map<DepartmentDto>(updatedDepartment);
            
            return ApiResponse<DepartmentDto>.Success(departmentDto, ErrorMessages.Department.UpdatedSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update department with ID: {DepartmentId}, Data: {@UpdateDepartmentDto}", id, updateDto);
            return ApiResponse<DepartmentDto>.Error(ErrorMessages.Department.UpdateFailed, 500);
        }
    }

    /// <summary>
    /// Performs soft delete on a department record, marking it as deleted without removing from database.
    /// The department will be excluded from normal queries but can be restored later if needed.
    /// Validates that the department has no active employees before deletion.
    /// </summary>
    /// <param name="id">The unique identifier of the department to delete</param>
    /// <returns>API response indicating success or failure of the delete operation</returns>
    public async Task<ApiResponse<bool>> DeleteDepartmentAsync(Guid id)
    {
        try
        {
            var result = await _departmentRepository.DeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.NotFound(ErrorMessages.Department.NotFoundGeneric);
            
            return ApiResponse<bool>.Success(true, ErrorMessages.Department.DeletedSuccessfully);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot delete department with ID: {DepartmentId} - has dependent records", id);
            return ApiResponse<bool>.ValidationError(new List<string> { ErrorMessages.Department.CannotDeleteWithEmployees });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete department with ID: {DepartmentId}", id);
            return ApiResponse<bool>.Error(ErrorMessages.Department.DeletionFailed, 500);
        }
    }

    /// <summary>
    /// Restores a previously soft-deleted department record, making it active again.
    /// Clears the deletion markers and makes the department visible in normal queries.
    /// </summary>
    /// <param name="id">The unique identifier of the department to restore</param>
    /// <returns>API response indicating success or failure of the restore operation</returns>
    public async Task<ApiResponse<bool>> RestoreDepartmentAsync(Guid id)
    {
        try
        {
            var result = await _departmentRepository.RestoreAsync(id);
            if (!result)
                return ApiResponse<bool>.NotFound(ErrorMessages.Department.NotFoundOrNotDeletedGeneric);
            
            return ApiResponse<bool>.Success(true, ErrorMessages.Department.RestoredSuccessfully);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore department with ID: {DepartmentId}", id);
            return ApiResponse<bool>.Error(ErrorMessages.Department.RestoreFailed, 500);
        }
    }

    /// <summary>
    /// Retrieves all departments that have been soft deleted from the system.
    /// These records are excluded from normal queries but retained for audit purposes and potential restoration.
    /// </summary>
    /// <returns>API response containing a list of deleted department records</returns>
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
            _logger.LogError(ex, "Failed to retrieve deleted departments");
            return ApiResponse<IEnumerable<DepartmentDto>>.Error(ErrorMessages.Department.RetrievalFailed, 500);
        }
    }
}