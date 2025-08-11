using System.ComponentModel.DataAnnotations;
using employee_management.Server.Models.Common;

namespace employee_management.Server.Models.Entities;

public class Department : IAuditable
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    // Navigation property
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    
    // IAuditable implementation
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
} 