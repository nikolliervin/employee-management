using Microsoft.EntityFrameworkCore;
using employee_management.Server.Data;
using employee_management.Server.Models.Entities;

namespace employee_management.Server.Data.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.Department)
            .ToListAsync();
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

        // Soft delete - the DbContext will handle this automatically
        _context.Employees.Remove(employee);
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

    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.Name.Contains(searchTerm) || e.Email.Contains(searchTerm))
            .ToListAsync();
    }
} 