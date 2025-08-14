using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

/// <summary>
/// Data transfer object for updating an existing department.
/// All fields are required to ensure complete department information.
/// </summary>
/// <example>
/// {
///   "name": "Engineering & Development",
///   "description": "Updated description for engineering team"
/// }
/// </example>
public class UpdateDepartmentDto
{
    /// <summary>
    /// The updated name of the department. Must be unique across all departments.
    /// </summary>
    /// <example>Engineering & Development</example>
    [Required(ErrorMessage = "Department name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The updated optional description of the department's purpose and responsibilities.
    /// </summary>
    /// <example>Updated description for engineering team responsible for product development</example>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}