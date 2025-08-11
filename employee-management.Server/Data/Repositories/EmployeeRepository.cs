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
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Employee>> SearchAsync(string searchTerm)
    {
        var normalizedSearchTerm = searchTerm.ToLower();
        
        return await _context.Employees
            .Include(e => e.Department)
            .Where(e => e.Name.ToLower().Contains(normalizedSearchTerm) || 
                       e.Email.ToLower().Contains(normalizedSearchTerm) ||
                       e.Department.Name.ToLower().Contains(normalizedSearchTerm))
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        employee.CreatedAt = DateTime.UtcNow;
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        var existingEmployee = await _context.Employees.FindAsync(employee.Id);
        if (existingEmployee == null)
            throw new InvalidOperationException($"Employee with ID {employee.Id} not found.");

        existingEmployee.Name = employee.Name;
        existingEmployee.Email = employee.Email;
        existingEmployee.DateOfBirth = employee.DateOfBirth;
        existingEmployee.DepartmentId = employee.DepartmentId;
        existingEmployee.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingEmployee;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _context.Employees
            .AnyAsync(e => e.Email == email && (!excludeId.HasValue || e.Id != excludeId.Value));
    }
} 