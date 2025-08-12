using Microsoft.EntityFrameworkCore;
using employee_management.Server.Data;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;
using employee_management.Server.Models.DTOs;

namespace employee_management.Server.Data.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;

    public DepartmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<Department>> GetAllAsync(PaginationRequest request)
    {
        var query = _context.Departments
            .Include(d => d.Employees.Where(e => !e.IsDeleted))
            .Where(d => !d.IsDeleted);

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(d => d.Name)
                : query.OrderBy(d => d.Name),
            "createdat" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(d => d.CreatedAt)
                : query.OrderBy(d => d.CreatedAt),
            _ => query.OrderBy(d => d.Name)
        };

        var totalCount = await query.CountAsync();
        var departments = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResult<Department>(departments, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<PaginatedResult<Department>> SearchAsync(SearchRequest request)
    {
        var query = _context.Departments
            .Include(d => d.Employees.Where(e => !e.IsDeleted))
            .Where(d => !d.IsDeleted);

        // Apply search filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(d => 
                d.Name.Contains(request.SearchTerm) ||
                (d.Description != null && d.Description.Contains(request.SearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(d => d.Name.Contains(request.Name));
        }

        if (request.CreatedAtFrom.HasValue)
        {
            query = query.Where(d => d.CreatedAt >= request.CreatedAtFrom.Value);
        }

        if (request.CreatedAtTo.HasValue)
        {
            query = query.Where(d => d.CreatedAt <= request.CreatedAtTo.Value);
        }

        // Apply sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(d => d.Name)
                : query.OrderBy(d => d.Name),
            "createdat" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(d => d.CreatedAt)
                : query.OrderBy(d => d.CreatedAt),
            _ => query.OrderBy(d => d.Name)
        };

        var totalCount = await query.CountAsync();
        var departments = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResult<Department>(departments, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        return await _context.Departments
            .Include(d => d.Employees.Where(e => !e.IsDeleted))
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
    }

    public async Task<Department> CreateAsync(Department department)
    {
        department.Id = Guid.NewGuid();
        department.CreatedAt = DateTime.UtcNow;
        department.CreatedBy = "System"; // TODO: Get from current user context
        department.IsDeleted = false;

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Department> UpdateAsync(Department department)
    {
        department.UpdatedAt = DateTime.UtcNow;
        department.UpdatedBy = "System"; // TODO: Get from current user context

        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null || department.IsDeleted)
            return false;

        // Check if department has active employees
        var hasActiveEmployees = await _context.Employees
            .AnyAsync(e => e.DepartmentId == id && !e.IsDeleted);
        
        if (hasActiveEmployees)
            throw new InvalidOperationException("Cannot delete department with active employees");

        // Soft delete
        department.IsDeleted = true;
        department.DeletedAt = DateTime.UtcNow;
        department.DeletedBy = "System"; // TODO: Get from current user context
        department.UpdatedAt = DateTime.UtcNow;
        department.UpdatedBy = "System";

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreAsync(Guid id)
    {
        // Use IgnoreQueryFilters to include deleted records in the search
        var department = await _context.Departments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id);
            
        if (department == null || !department.IsDeleted)
            return false;

        // Restore the department
        department.IsDeleted = false;
        department.DeletedAt = null;
        department.DeletedBy = null;
        department.UpdatedAt = DateTime.UtcNow;
        department.UpdatedBy = "System"; // TODO: Get from current user context
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Department>> GetDeletedAsync()
    {
        return await _context.Departments
            .IgnoreQueryFilters()
            .Include(d => d.Employees)
            .Where(d => d.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Departments
            .AnyAsync(d => d.Id == id && !d.IsDeleted);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null)
    {
        var query = _context.Departments
            .Where(d => !d.IsDeleted && d.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(d => d.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}