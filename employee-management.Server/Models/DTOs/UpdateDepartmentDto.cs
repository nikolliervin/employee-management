using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

public class UpdateDepartmentDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
}