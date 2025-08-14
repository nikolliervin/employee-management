namespace employee_management.Server.Models.DTOs;

/// <summary>
/// Data transfer object representing an employee with complete information including audit fields.
/// This DTO is used for returning employee data in API responses.
/// </summary>
/// <example>
/// {
///   "id": "123e4567-e89b-12d3-a456-426614174000",
///   "name": "John Doe",
///   "email": "john.doe@example.com",
///   "dateOfBirth": "1990-01-01T00:00:00",
///   "departmentId": "456e7890-e89b-12d3-a456-426614174000",
///   "departmentName": "Engineering",
///   "createdAt": "2024-01-15T10:30:00Z",
///   "createdBy": "system",
///   "updatedAt": "2024-01-20T15:45:00Z",
///   "updatedBy": "admin"
/// }
/// </example>
public class EmployeeDto
{
    /// <summary>
    /// The unique identifier (GUID) of the employee.
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The full name of the employee.
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The email address of the employee. This is unique across all employees.
    /// </summary>
    /// <example>john.doe@example.com</example>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The date of birth of the employee.
    /// </summary>
    /// <example>1990-01-01T00:00:00</example>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// The unique identifier (GUID) of the department where the employee works.
    /// </summary>
    /// <example>456e7890-e89b-12d3-a456-426614174000</example>
    public Guid DepartmentId { get; set; }
    
    /// <summary>
    /// The name of the department where the employee works.
    /// </summary>
    /// <example>Engineering</example>
    public string DepartmentName { get; set; } = string.Empty;
    
    /// <summary>
    /// The timestamp when the employee record was created.
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that created the employee record.
    /// </summary>
    /// <example>system</example>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// The timestamp when the employee record was last updated. Null if never updated.
    /// </summary>
    /// <example>2024-01-20T15:45:00Z</example>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that last updated the employee record. Null if never updated.
    /// </summary>
    /// <example>admin</example>
    public string? UpdatedBy { get; set; }
} 