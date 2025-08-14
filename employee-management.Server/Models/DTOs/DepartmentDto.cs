namespace employee_management.Server.Models.DTOs;

/// <summary>
/// Data transfer object representing a department with complete information including audit fields and employee count.
/// This DTO is used for returning department data in API responses.
/// </summary>
/// <example>
/// {
///   "id": "456e7890-e89b-12d3-a456-426614174000",
///   "name": "Engineering",
///   "description": "Software development and engineering team",
///   "employeeCount": 25,
///   "createdAt": "2024-01-01T00:00:00Z",
///   "createdBy": "system",
///   "updatedAt": "2024-01-20T15:45:00Z",
///   "updatedBy": "admin"
/// }
/// </example>
public class DepartmentDto
{
    /// <summary>
    /// The unique identifier (GUID) of the department.
    /// </summary>
    /// <example>456e7890-e89b-12d3-a456-426614174000</example>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The name of the department. This is unique across all departments.
    /// </summary>
    /// <example>Engineering</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The optional description of the department's purpose and responsibilities.
    /// </summary>
    /// <example>Software development and engineering team responsible for product development</example>
    public string? Description { get; set; }
    
    /// <summary>
    /// The current number of active employees working in this department.
    /// </summary>
    /// <example>25</example>
    public int EmployeeCount { get; set; }
    
    /// <summary>
    /// The timestamp when the department record was created.
    /// </summary>
    /// <example>2024-01-01T00:00:00Z</example>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that created the department record.
    /// </summary>
    /// <example>system</example>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// The timestamp when the department record was last updated. Null if never updated.
    /// </summary>
    /// <example>2024-01-20T15:45:00Z</example>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that last updated the department record. Null if never updated.
    /// </summary>
    /// <example>admin</example>
    public string? UpdatedBy { get; set; }
}