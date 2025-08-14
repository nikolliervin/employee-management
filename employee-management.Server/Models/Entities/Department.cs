using System.ComponentModel.DataAnnotations;
using employee_management.Server.Models.Common;

namespace employee_management.Server.Models.Entities;

/// <summary>
/// Entity representing a department in the system.
/// Implements IAuditable for tracking creation, updates, and soft delete operations.
/// </summary>
/// <example>
/// {
///   "id": "456e7890-e89b-12d3-a456-426614174000",
///   "name": "Engineering",
///   "description": "Software development and engineering team",
///   "employees": [
///     { "id": "123e4567-e89b-12d3-a456-426614174000", "name": "John Doe" }
///   ],
///   "createdAt": "2024-01-01T00:00:00Z",
///   "createdBy": "system",
///   "updatedAt": "2024-01-20T15:45:00Z",
///   "updatedBy": "admin",
///   "deletedAt": null,
///   "deletedBy": null,
///   "isDeleted": false,
///   "rowVersion": [1, 0, 0, 0]
/// }
/// </example>
public class Department : IAuditable
{
    /// <summary>
    /// The unique identifier (GUID) of the department.
    /// </summary>
    /// <example>456e7890-e89b-12d3-a456-426614174000</example>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The name of the department. Must be unique across all departments.
    /// </summary>
    /// <example>Engineering</example>
    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The optional description of the department's purpose and responsibilities.
    /// </summary>
    /// <example>Software development and engineering team responsible for product development</example>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Navigation property to the collection of employees working in this department.
    /// </summary>
    /// <example>[{ "id": "123e4567-e89b-12d3-a456-426614174000", "name": "John Doe" }]</example>
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    
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
    
    /// <summary>
    /// The timestamp when the department record was soft-deleted. Null if not deleted.
    /// </summary>
    /// <example>null</example>
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// The identifier of the user or system that soft-deleted the department record. Null if not deleted.
    /// </summary>
    /// <example>null</example>
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// Indicates whether the department record has been soft-deleted.
    /// </summary>
    /// <example>false</example>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control.
    /// </summary>
    /// <example>[1, 0, 0, 0]</example>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
} 
