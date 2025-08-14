using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

/// <summary>
/// Request model for searching employees with various filters and pagination support.
/// Inherits from PaginationRequest to include pagination parameters.
/// </summary>
/// <example>
/// {
///   "searchTerm": "john",
///   "pageNumber": 1,
///   "pageSize": 10,
///   "departmentId": "456e7890-e89b-12d3-a456-426614174000",
///   "dateOfBirthFrom": "1980-01-01",
///   "dateOfBirthTo": "2000-12-31"
/// }
/// </example>
public class SearchRequest : PaginationRequest
{
    /// <summary>
    /// General search term that will search across multiple fields (name, email, etc.).
    /// </summary>
    /// <example>john</example>
    [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Filter by specific employee name (exact or partial match).
    /// </summary>
    /// <example>John Doe</example>
    public string? Name { get; set; }
    
    /// <summary>
    /// Filter by specific email address (exact or partial match).
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string? Email { get; set; }
    
    /// <summary>
    /// Filter employees born after this date (inclusive).
    /// </summary>
    /// <example>1980-01-01</example>
    public DateTime? DateOfBirthFrom { get; set; }
    
    /// <summary>
    /// Filter employees born before this date (inclusive).
    /// </summary>
    /// <example>2000-12-31</example>
    public DateTime? DateOfBirthTo { get; set; }
    
    /// <summary>
    /// Filter employees by specific department ID.
    /// </summary>
    /// <example>456e7890-e89b-12d3-a456-426614174000</example>
    public Guid? DepartmentId { get; set; }
    
    /// <summary>
    /// Filter employees created after this date (inclusive).
    /// </summary>
    /// <example>2024-01-01</example>
    public DateTime? CreatedAtFrom { get; set; }
    
    /// <summary>
    /// Filter employees created before this date (inclusive).
    /// </summary>
    /// <example>2024-12-31</example>
    public DateTime? CreatedAtTo { get; set; }
} 