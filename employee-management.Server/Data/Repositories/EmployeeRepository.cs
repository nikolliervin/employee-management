using Microsoft.EntityFrameworkCore;
using employee_management.Server.Data;
using employee_management.Server.Data.QueryBuilders;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;
using employee_management.Server.Models.DTOs;

namespace employee_management.Server.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly QueryBuilder _queryBuilder;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
        _queryBuilder = new QueryBuilder(context);
    }

    public async Task<PaginatedResult<Employee>> GetAllAsync(PaginationRequest request)
    {
        var query = _queryBuilder.BuildBaseQuery();
        query = _queryBuilder.ApplySorting(query, request.SortBy, request.SortOrder);

        var countQuery = query.CountAsync();
        var dataQuery = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
            
        await Task.WhenAll(countQuery, dataQuery);

        var totalCount = await countQuery;
        var employees = await dataQuery;

        return new PaginatedResult<Employee>(employees, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<PaginatedResult<Employee>> SearchAsync(SearchRequest request)
    {
        // Build query using query builder
        var query = _queryBuilder.BuildSearchQuery(request);
        
        // Execute count and data queries simultaneously
        var countQuery = query.CountAsync();
        var dataQuery = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // Both queries execute simultaneously
        await Task.WhenAll(countQuery, dataQuery);

        var totalCount = await countQuery;
        var employees = await dataQuery;

        return new PaginatedResult<Employee>(employees, totalCount, request.PageNumber, request.PageSize);
    }





    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return false;

        // Soft delete - mark as deleted instead of removing
        employee.IsDeleted = true;
        employee.DeletedAt = DateTime.UtcNow;
        employee.DeletedBy = "System"; // TODO: Get from current user context
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email && e.Id != excludeId.Value);
        }
        
        return await _context.Employees.AnyAsync(e => e.Email == email);
    }

    public async Task<IEnumerable<Employee>> GetDeletedAsync()
    {
        return await _context.Employees
            .IgnoreQueryFilters()
            .Include(e => e.Department)
            .Where(e => e.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> RestoreAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null || !employee.IsDeleted)
            return false;

        // Restore the employee
        employee.IsDeleted = false;
        employee.DeletedAt = null;
        employee.DeletedBy = null;
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = "System"; // TODO: Get from current user context
        
        await _context.SaveChangesAsync();
        return true;
    }
} 