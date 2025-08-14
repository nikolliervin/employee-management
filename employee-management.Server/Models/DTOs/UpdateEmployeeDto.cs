using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

/// <summary>
/// Data transfer object for updating an existing employee.
/// All fields are required to ensure complete employee information.
/// </summary>
/// <example>
/// {
///   "name": "John Doe Updated",
///   "email": "john.doe.updated@example.com",
///   "dateOfBirth": "1990-01-01",
///   "departmentId": "456e7890-e89b-12d3-a456-426614174000"
/// }
/// </example>
public class UpdateEmployeeDto
{
    /// <summary>
    /// The updated full name of the employee.
    /// </summary>
    /// <example>John Doe Updated</example>
    [Required(ErrorMessage = "Employee name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The updated email address of the employee. Must be unique across all employees.
    /// </summary>
    /// <example>john.doe.updated@example.com</example>
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(255, ErrorMessage = "Email address cannot exceed 255 characters")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// The updated date of birth of the employee. Employee must be at least 18 years old.
    /// </summary>
    /// <example>1990-01-01</example>
    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// The updated unique identifier of the department where the employee will work.
    /// </summary>
    /// <example>456e7890-e89b-12d3-a456-426614174000</example>
    [Required(ErrorMessage = "Department ID is required")]
    public Guid DepartmentId { get; set; }
} 