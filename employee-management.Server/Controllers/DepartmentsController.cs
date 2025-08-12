using Microsoft.AspNetCore.Mvc;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Responses;
using employee_management.Server.Services;

namespace employee_management.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // GET: api/v1/departments
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PaginatedResult<DepartmentDto>>>> GetDepartments([FromQuery] PaginationRequest pagination)
    {
        var response = await _departmentService.GetAllDepartmentsAsync(pagination);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/v1/departments/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> GetDepartment(Guid id)
    {
        var response = await _departmentService.GetDepartmentByIdAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/v1/departments/search
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<DepartmentDto>>>> SearchDepartments([FromQuery] SearchRequest request)
    {
        var response = await _departmentService.SearchDepartmentsAsync(request);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/v1/departments/deleted
    [HttpGet("deleted")]
    public async Task<ActionResult<ApiResponse<IEnumerable<DepartmentDto>>>> GetDeletedDepartments()
    {
        var response = await _departmentService.GetDeletedDepartmentsAsync();
        return StatusCode(response.StatusCode, response);
    }

    // POST: api/v1/departments
    [HttpPost]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> CreateDepartment(CreateDepartmentDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            var validationResponse = ApiResponse<DepartmentDto>.ValidationError(errors);
            return StatusCode(validationResponse.StatusCode, validationResponse);
        }

        var response = await _departmentService.CreateDepartmentAsync(createDto);
        return StatusCode(response.StatusCode, response);
    }

    // PUT: api/v1/departments/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DepartmentDto>>> UpdateDepartment(Guid id, UpdateDepartmentDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            var validationResponse = ApiResponse<DepartmentDto>.ValidationError(errors);
            return StatusCode(validationResponse.StatusCode, validationResponse);
        }

        var response = await _departmentService.UpdateDepartmentAsync(id, updateDto);
        return StatusCode(response.StatusCode, response);
    }

    // DELETE: api/v1/departments/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteDepartment(Guid id)
    {
        var response = await _departmentService.DeleteDepartmentAsync(id);
        return StatusCode(response.StatusCode, response);
    }

    // POST: api/v1/departments/{id}/restore
    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult<ApiResponse<bool>>> RestoreDepartment(Guid id)
    {
        var response = await _departmentService.RestoreDepartmentAsync(id);
        return StatusCode(response.StatusCode, response);
    }
}