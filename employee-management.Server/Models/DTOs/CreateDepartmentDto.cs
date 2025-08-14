using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

/// <summary>
/// Data transfer object for creating a new department.
/// </summary>
/// <example>
/// {
///   "name": "Engineering",
///   "description": "Software development and engineering team"
/// }
/// </example>
public class CreateDepartmentDto
{
    /// <summary>
    /// The name of the department. Must be unique across all departments.
    /// </summary>
    /// <example>Engineering</example>
    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional description of the department's purpose and responsibilities.
    /// </summary>
    /// <example>Software development and engineering team responsible for product development</example>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}