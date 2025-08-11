using Microsoft.EntityFrameworkCore;
using employee_management.Server.Data;
using employee_management.Server.Models.Entities;
using employee_management.Server.Models.Responses;

namespace employee_management.Server.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<Employee>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? sortBy = "Name", string sortOrder = "asc")
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .AsQueryable();

        // Apply sorting
        query = ApplySorting(query, sortBy, sortOrder);

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var employees = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Employee>(employees, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedResult<Employee>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 10, string? sortBy = "Name", string sortOrder = "asc")
    {
        var query = _context.Employees
            .Include(e => e.Department)
            .Where(e => e.Name.Contains(searchTerm) || e.Email.Contains(searchTerm))
            .AsQueryable();

        // Apply sorting
        query = ApplySorting(query, sortBy, sortOrder);

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var employees = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Employee>(employees, totalCount, pageNumber, pageSize);
    }

    private IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, string sortOrder)
    {
        return sortBy?.ToLower() switch
        {
            "name" => sortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(e => e.Name)
                : query.OrderBy(e => e.Name),
            "email" => sortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(e => e.Email)
                : query.OrderBy(e => e.Email),
            "dateofbirth" => sortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(e => e.DateOfBirth)
                : query.OrderBy(e => e.DateOfBirth),
            "createdat" => sortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(e => e.CreatedAt)
                : query.OrderBy(e => e.CreatedAt),
            "department" => sortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(e => e.Department.Name)
                : query.OrderBy(e => e.Department.Name),
            _ => query.OrderBy(e => e.Name) // Default sorting
        };
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