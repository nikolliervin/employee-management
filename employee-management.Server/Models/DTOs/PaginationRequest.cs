using System.ComponentModel.DataAnnotations;

namespace employee_management.Server.Models.DTOs;

public class PaginationRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;
    
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
    
    public string? SortBy { get; set; } = "Name";
    
    [RegularExpression("^(asc|desc)$", ErrorMessage = "Sort order must be 'asc' or 'desc'")]
    public string SortOrder { get; set; } = "asc";
} 