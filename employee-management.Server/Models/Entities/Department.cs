using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.Entities;

public class Department
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    // Navigation property
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
} 