using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

public class SearchRequest : PaginationRequest
{
    [MaxLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    public string? SearchTerm { get; set; }
    
    // Additional search filters
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirthFrom { get; set; }
    public DateTime? DateOfBirthTo { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
} 