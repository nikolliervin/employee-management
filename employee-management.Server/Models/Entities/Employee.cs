using System.ComponentModel.DataAnnotations;
using employee_management.Server.Models.Common;

namespace employee_management.Server.Models.Entities;

/// <summary>
/// Entity representing an employee in the system.
/// Implements IAuditable for tracking creation, updates, and soft delete operations.
/// </summary>
/// <example>
/// {
///   "id": "123e4567-e89b-12d3-a456-426614174000",
///   "name": "John Doe",
///   "email": "john.doe@example.com",
///   "dateOfBirth": "1990-01-01T00:00:00",
///   "departmentId": "456e7890-e89b-12d3-a456-426614174000",
///   "department": { ... },
///   "createdAt": "2024-01-15T10:30:00Z",
///   "createdBy": "system",
///   "updatedAt": "2024-01-20T15:45:00Z",
///   "updatedBy": "admin",
///   "deletedAt": null,
///   "deletedBy": null,
///   "isDeleted": false,
///   "rowVersion": [1, 0, 0, 0]
/// }
/// </example>
public class Employee : IAuditable
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
    [Required(ErrorMessage = "Employee name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The email address of the employee. Must be unique across all employees.
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(255, ErrorMessage = "Email address cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The date of birth of the employee. Employee must be at least 18 years old.
    /// </summary>
    /// <example>1990-01-01T00:00:00</example>
    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// The unique identifier (GUID) of the department where the employee works.
    /// </summary>
    /// <example>456e7890-e89b-12d3-a456-426614174000</example>
    public Guid DepartmentId { get; set; }
    
    /// <summary>
    /// Navigation property to the department where the employee works.
    /// </summary>
    /// <example>{ "id": "456e7890-e89b-12d3-a456-426614174000", "name": "Engineering" }</example>
    public virtual Department Department { get; set; } = null!;
    
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
    
    /// <summary>
    /// The timestamp when the employee record was soft-deleted. Null if not deleted.
    /// </summary>
    /// <example>null</example>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that soft-deleted the employee record. Null if not deleted.
    /// </summary>
    /// <example>null</example>
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// Indicates whether the employee record has been soft-deleted.
    /// </summary>
    /// <example>false</example>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control.
    /// </summary>
    /// <example>[1, 0, 0, 0]</example>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
} 
