using Microsoft.EntityFrameworkCore;
using employee_management.Server.Data;
using employee_management.Server.Models.DTOs;
using employee_management.Server.Models.Entities;

namespace employee_management.Server.Data.QueryBuilders;

public class QueryBuilder
{
    private readonly ApplicationDbContext _context;

    public QueryBuilder(ApplicationDbContext context)
    {
        _context = context;
    }

    // Build base query with includes
    public IQueryable<Employee> BuildBaseQuery()
    {
        return _context.Employees
            .Include(e => e.Department)
            .AsQueryable();
    }

    // Build complete search query
    public IQueryable<Employee> BuildSearchQuery(SearchRequest request)
    {
        var query = BuildBaseQuery();
        
        // Apply all filters
        query = ApplySearchFilters(query, request);
        
        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);
        
        return query;
    }

    // Apply search filters
    private IQueryable<Employee> ApplySearchFilters(IQueryable<Employee> query, SearchRequest request)
    {
        // Text search using StartsWith for index optimization
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(e => 
                e.Name.StartsWith(request.SearchTerm) || 
                e.Email.StartsWith(request.SearchTerm));
        }

        // Specific field filters
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(e => e.Name.StartsWith(request.Name));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            query = query.Where(e => e.Email.StartsWith(request.Email));
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
        }

        // Date range filters using date indexes
        if (request.DateOfBirthFrom.HasValue)
        {
            query = query.Where(e => e.DateOfBirth >= request.DateOfBirthFrom.Value);
        }

        if (request.DateOfBirthTo.HasValue)
        {
            query = query.Where(e => e.DateOfBirth <= request.DateOfBirthTo.Value);
        }

        if (request.CreatedAtFrom.HasValue)
        {
            query = query.Where(e => e.CreatedAt >= request.CreatedAtFrom.Value);
        }

        if (request.CreatedAtTo.HasValue)
        {
            query = query.Where(e => e.CreatedAt <= request.CreatedAtTo.Value);
        }

        return query;
    }

    // Apply sorting
    public IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, string sortOrder)
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

    // Build query for specific use cases
    public IQueryable<Employee> BuildDepartmentQuery(Guid departmentId)
    {
        return BuildBaseQuery()
            .Where(e => e.DepartmentId == departmentId);
    }

    public IQueryable<Employee> BuildDateRangeQuery(DateTime from, DateTime to)
    {
        return BuildBaseQuery()
            .Where(e => e.CreatedAt >= from && e.CreatedAt <= to);
    }

    public IQueryable<Employee> BuildAgeRangeQuery(int minAge, int maxAge)
    {
        var maxDate = DateTime.Today.AddYears(-minAge);
        var minDate = DateTime.Today.AddYears(-maxAge);
        
        return BuildBaseQuery()
            .Where(e => e.DateOfBirth >= minDate && e.DateOfBirth <= maxDate);
    }
} 