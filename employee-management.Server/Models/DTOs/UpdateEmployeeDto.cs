using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

public class UpdateEmployeeDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Required]
    public Guid DepartmentId { get; set; }
} 