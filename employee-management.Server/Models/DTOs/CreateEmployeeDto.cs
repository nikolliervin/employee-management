using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

public class CreateEmployeeDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateOfBirth { get; set; }
    
    [Required]
    public int DepartmentId { get; set; }
} 